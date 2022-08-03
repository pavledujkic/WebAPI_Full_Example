using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI_Full_Example.Controllers;

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
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
    {
        _logger.LogInfo("GetEmployeesForCompany was called");

        Company? company = await _repository.Company.GetCompanyAsync(companyId, false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

            return NotFound();
        }

        _logger.LogInfo($"Company with id: {companyId} found in the database.");

        var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, false);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return Ok(employeesDto);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
    {
        Company? company = await _repository.Company.GetCompanyAsync(companyId, false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

            return NotFound();
        }

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
    public Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto? employee)
    {
        if (employee == null)
        {
            _logger.LogError("EmployeeForCreationDto object sent from client is null.");

            return Task.FromResult<IActionResult>(BadRequest("EmployeeForCreationDto object is null"));
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the EmployeeForCreationDto object");

            return Task.FromResult<IActionResult>(UnprocessableEntity(ModelState));
        }

        return EmployeeForCompany(companyId, employee);
    }

    private async Task<IActionResult> EmployeeForCompany(Guid companyId, EmployeeForCreationDto employee)
    {
        Company? company = await _repository.Company.GetCompanyAsync(companyId, false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

            return NotFound();
        }

        var employeeEntity = _mapper.Map<Employee>(employee);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return CreatedAtRoute("GetEmployeeForCompany",
            new { companyId, id = employeeToReturn.Id }, employeeToReturn);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        Company? company = await _repository.Company.GetCompanyAsync(companyId, false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

            return NotFound();
        }

        Employee? employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, false);

        if (employeeForCompany == null)
        {
            _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");

            return NotFound();
        }

        _repository.Employee.DeleteEmployee(employeeForCompany);

        await _repository.SaveAsync();

        _logger.LogInfo($"Employee with id: {id} was deleted from the database.");

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto? employee)
    {
        if (employee == null)
        {
            _logger.LogError("EmployeeForUpdateDto object sent from client is null.");

            return Task.FromResult<IActionResult>(BadRequest("EmployeeForUpdateDto object is null"));
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");

            return Task.FromResult<IActionResult>(UnprocessableEntity(ModelState));
        }

        return ActionResult(companyId, id, employee);
    }

    private async Task<IActionResult> ActionResult(Guid companyId, Guid id, EmployeeForUpdateDto employee)
    {
        Company? company = await _repository.Company.GetCompanyAsync(companyId, false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

            return NotFound();
        }

        Employee? employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, true);

        if (employeeEntity == null)
        {
            _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");

            return NotFound();
        }

        _mapper.Map(employee, employeeEntity);

        await _repository.SaveAsync();

        _logger.LogInfo($"Employee with id: {id} was updated in the database.");

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto>? patchDoc)
    {
        if (patchDoc != null) return UnprocessableEntityObjectResult(companyId, id, patchDoc);

        _logger.LogError("JsonPatchDocument object sent from client is null.");

        return Task.FromResult<IActionResult>(BadRequest("JsonPatchDocument object is null"));

    }

    private async Task<IActionResult> UnprocessableEntityObjectResult(Guid companyId, Guid id, JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
    {
        Company? company = await _repository.Company.GetCompanyAsync(companyId, false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

            return NotFound();
        }

        Employee? employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, true);

        if (employeeEntity == null)
        {
            _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");

            return NotFound();
        }

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

        patchDoc.ApplyTo(employeeToPatch, ModelState);

        TryValidateModel(employeeToPatch);

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model object sent from client.");

            return UnprocessableEntity(ModelState);
        }

        _mapper.Map(employeeToPatch, employeeEntity);

        await _repository.SaveAsync();

        _logger.LogInfo($"Employee with id: {id} was updated in the database.");

        return NoContent();
    }
}