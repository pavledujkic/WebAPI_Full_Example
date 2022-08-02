using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI_Full_Example.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesControler : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public EmployeesControler(ILoggerManager logger, IRepositoryManager repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            _logger.LogInfo("GetEmployeesForCompany was called");

            Company? company = _repository.Company.GetCompany(companyId, false);
            
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                
                return NotFound();
            }

            _logger.LogInfo($"Company with id: {companyId} found in the database.");

            var employeesFromDb = _repository.Employee.GetEmployees(companyId, false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            
            return Ok(employeesDto);
        }

        [HttpGet("{id:guid}", Name="GetEmployeeForCompany")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            Company? company = _repository.Company.GetCompany(companyId, false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

                return NotFound();
            }

            Employee? employeeDb = _repository.Employee.GetEmployee(companyId, id, false);

            if (employeeDb == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");

                return NotFound();
            }

            _logger.LogInfo($"Employee with id: {id} found in the database.");

            var employeeDto = _mapper.Map<EmployeeDto>(employeeDb);

            return Ok(employeeDto);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto? employee)
        {
            if (employee == null)
            {
                _logger.LogError("EmployeeForCreationDto object sent from client is null.");
                
                return BadRequest("EmployeeForCreationDto object is null");
            }

            Company? company = _repository.Company.GetCompany(companyId, false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

                return NotFound();
            }

            var employeeEntity = _mapper.Map<Employee>(employee);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

            _repository.Save();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return CreatedAtRoute("GetEmployeeForCompany", 
                new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }
    }
}
