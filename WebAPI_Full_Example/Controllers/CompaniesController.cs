using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{id:guid}")]
    public IActionResult GetCompany(Guid id)
    {
        var company = _repository.Company.GetCompany(id, false);
        
        if (company == null)
        {
            _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
            
            return NotFound();
        }

        _logger.LogInfo($"Company with id: {id} found in the database.");
        
        var companyDto = _mapper.Map<CompanyDto>(company);
        
        return Ok(companyDto);
    }
}