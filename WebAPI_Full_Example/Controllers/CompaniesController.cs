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
    public IActionResult GetCompanies()
    {
        _logger.LogInfo("GetCompanies was called");

        var companies = _repository.Company.GetAllCompanies(false);
        
        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        _logger.LogInfo("Companies got from the database.");

        return Ok(companiesDto);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    public IActionResult GetCompany(Guid id)
    {
        Company? company = _repository.Company.GetCompany(id, false);
        
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
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto? company)
    {
        if (company == null)
        {
            _logger.LogError("CompanyForCreationDto object sent from client is null.");

            return BadRequest("CompanyForCreationDto object is null");
        }

        var companyEntity = _mapper.Map<Company>(company);

        _repository.Company.CreateCompany(companyEntity);

        _repository.Save();

        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

        return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid>? ids)
    {
        if (ids == null)
        {
            _logger.LogError("Parameter ids is null"); 
            
            return BadRequest("Parameter ids is null");
        }

        var idsList = ids.ToList();
        
        var companyEntities = _repository.Company.GetByIds(idsList, trackChanges: false);

        if (idsList.Count != companyEntities.Count())
        {
            _logger.LogError("Some ids are not valid in a collection"); 
            
            return NotFound();
        }
        
        var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities); 
        
        return Ok(companiesToReturn);
    }

    [HttpPost("collection")]
    public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection == null)
        {
            _logger.LogError("Company collection sent from client is null.");

            return BadRequest("Company collection is null");
        }

        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }

        _repository.Save();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

        return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
    }
}