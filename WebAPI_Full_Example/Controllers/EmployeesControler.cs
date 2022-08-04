using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAPI_Full_Example.ActionFilters;

namespace WebAPI_Full_Example.Controllers;

[Route("api/companies/{companyId:guid}/employees")]
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
    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
        [FromQuery] EmployeeParameters employeeParameters)
    {
        _logger.LogInfo($"Company with id: {companyId} found in the database.");

        var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, 
            employeeParameters, false);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return Ok(employeesDto);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
    {
        Employee? employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, false);

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
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId,
        [FromBody] EmployeeForCreationDto? employee)
    {
        var employeeEntity = _mapper.Map<Employee>(employee);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return CreatedAtRoute("GetEmployeeForCompany",
            new { companyId, id = employeeToReturn.Id }, employeeToReturn);
    }

    [HttpDelete("{id:guid}")]
    [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        var employeeForCompany = HttpContext.Items["employee"] as Employee;

        _repository.Employee.DeleteEmployee(employeeForCompany!);

        await _repository.SaveAsync();

        _logger.LogInfo($"Employee with id: {id} was deleted from the database.");

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] EmployeeForUpdateDto? employee)
    {
        var employeeEntity = HttpContext.Items["employee"] as Employee;

        _mapper.Map(employee, employeeEntity);

        await _repository.SaveAsync();

        _logger.LogInfo($"Employee with id: {id} was updated in the database.");

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
    public Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto>? patchDoc)
    {
        if (patchDoc == null)
        {
            _logger.LogError("patchDoc object sent from client is null.");

            return Task.FromResult<IActionResult>(BadRequest("patchDoc object is null"));
        }

        var employeeEntity = HttpContext.Items["employee"] as Employee;

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

        patchDoc.ApplyTo(employeeToPatch, ModelState);

        TryValidateModel(employeeToPatch);

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model object sent from client.");

            return Task.FromResult<IActionResult>(UnprocessableEntity(ModelState));
        }

        _mapper.Map(employeeToPatch, employeeEntity);

        return ActionResult(id);
    }

    private async Task<IActionResult> ActionResult(Guid id)
    {
        await _repository.SaveAsync();

        _logger.LogInfo($"Employee with id: {id} was updated in the database.");

        return NoContent();
    }
}