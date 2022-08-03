using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI_Full_Example.ModelBinders;

namespace WebAPI_Full_Example.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly ILoggerManager _logger;
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public CompaniesController(ILoggerManager logger, IRepositoryManager repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        _logger.LogInfo("GetCompanies was called");

        var companies = await _repository.Company.GetAllCompaniesAsync(false);

        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        _logger.LogInfo("Companies got from the database.");

        return Ok(companiesDto);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        Company? company = await _repository.Company.GetCompanyAsync(id, false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

            return NotFound();
        }

        _logger.LogInfo($"Company with id: {id} found in the database.");

        var companyDto = _mapper.Map<CompanyDto>(company);

        return Ok(companyDto);
    }

    [HttpPost]
    public Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto? company)
    {
        if (company == null)
        {
            _logger.LogError("CompanyForCreationDto object sent from client is null.");

            return Task.FromResult<IActionResult>(BadRequest("CompanyForCreationDto object is null"));
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in the companyForCreationDto object sent from client.");

            return Task.FromResult<IActionResult>(UnprocessableEntity(ModelState));
        }

        var companyEntity = _mapper.Map<Company>(company);

        _repository.Company.CreateCompany(companyEntity);

        return ActionResult(companyEntity);
    }

    private async Task<IActionResult> ActionResult(Company companyEntity)
    {
        await _repository.SaveAsync();

        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

        return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid>? ids)
    {
        if (ids == null)
        {
            _logger.LogError("Parameter ids is null");

            return Task.FromResult<IActionResult>(BadRequest("Parameter ids is null"));
        }

        var idsList = ids.ToList();

        return CompanyCollection(idsList);
    }

    private async Task<IActionResult> CompanyCollection(IReadOnlyCollection<Guid> idsList)
    {
        var companyEntities = await _repository.Company.GetByIdsAsync(idsList, trackChanges: false);

        if (idsList.Count != companyEntities.Count())
        {
            _logger.LogError("Some ids are not valid in a collection");

            return NotFound();
        }

        var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

        return Ok(companiesToReturn);
    }

    [HttpPost("collection")]
    public Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto>? companyCollection)
    {
        if (companyCollection == null)
        {
            _logger.LogError("Company collection sent from client is null.");

            return Task.FromResult<IActionResult>(BadRequest("Company collection is null"));
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in the company collection sent from client.");

            return Task.FromResult<IActionResult>(UnprocessableEntity(ModelState));
        }

        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

        var companyEntitiesList = companyEntities.ToList();

        foreach (Company company in companyEntitiesList)
        {
            _repository.Company.CreateCompany(company);
        }

        return CreatedAtRouteResult(companyEntitiesList);
    }

    private async Task<IActionResult> CreatedAtRouteResult(IEnumerable<Company> companyEntities)
    {
        await _repository.SaveAsync();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

        return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        Company? company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

            return NotFound();
        }

        _repository.Company.DeleteCompany(company);

        await _repository.SaveAsync();

        _logger.LogInfo($"Company with id: {id} was deleted from the database.");

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto? company)
    {
        if (company == null)
        {
            _logger.LogError("CompanyForUpdateDto object sent from client is null.");

            return Task.FromResult<IActionResult>(BadRequest("CompanyForUpdateDto object is null"));
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in the companyForUpdateDto object sent from client.");

            return Task.FromResult<IActionResult>(UnprocessableEntity(ModelState));
        }

        return NoContentResult(id, company);
    }

    private async Task<IActionResult> NoContentResult(Guid id, CompanyForUpdateDto company)
    {
        Company? companyEntity = await _repository.Company.GetCompanyAsync(id, trackChanges: true);

        if (companyEntity == null)
        {
            _logger.LogError($"Company with id: {id} doesn't exist in the database.");

            return NotFound();
        }

        _mapper.Map(company, companyEntity);

        await _repository.SaveAsync();

        _logger.LogInfo($"Company with id: {id} was updated in the database.");

        return NoContent();
    }
}