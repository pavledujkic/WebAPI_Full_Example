using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId:guid}/employees")]
[ApiController]
public class EmployeesControler : ControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesControler(IServiceManager service) => _service = service;

    [HttpGet(Name = "GetEmployeesForCompany")]
    [HttpHead]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
        [FromQuery] EmployeeParameters employeeParameters)
    {
        var linkParams = new LinkParameters(employeeParameters, HttpContext);
        
        (LinkResponse? linkResponse, MetaData? metaData) = 
            await _service.EmployeeService.GetEmployeesAsync(companyId, 
                linkParams, trackChanges: false);

        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(metaData));

        return linkResponse.HasLinks ? 
            Ok(linkResponse.LinkedEntities) :
            Ok(linkResponse.ShapedEntities);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
    {
        EmployeeDto employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id,
            trackChanges: false);

        return Ok(employee);
    }

    [HttpPost(Name = "CreateEmployeeForCompany")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId,
        [FromBody] EmployeeForCreationDto? employee)
    {
        EmployeeDto employeeToReturn = await
            _service.EmployeeService.CreateEmployeeForCompany(companyId, employee!,
                trackChanges: false);

        return CreatedAtRoute("GetEmployeeForCompany",
            new { companyId, id = employeeToReturn.Id }, employeeToReturn);
    }

    [HttpDelete("{id:guid}", Name = "DeleteEmployeeForCompany")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges:
             false);

        return NoContent();
    }

    [HttpPut("{id:guid}", Name = "UpdateEmployeeForCompany")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] EmployeeForUpdateDto? employee)
    {
        await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee!,
             compTrackChanges: false, empTrackChanges: true);

        return NoContent();
    }

    [HttpPatch("{id:guid}", Name = "PartiallyUpdateEmployeeForCompany")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany
    (Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto>? patchDoc)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null.");

        (EmployeeForUpdateDto? employeeToPatch, Employee? employeeEntity) =
            await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id,
            compTrackChanges: false, empTrackChanges: true);

        patchDoc.ApplyTo(employeeToPatch, ModelState);

        TryValidateModel(employeeToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.EmployeeService.SaveChangesForPatchAsync(employeeToPatch,
            employeeEntity);

        return NoContent();
    }
}