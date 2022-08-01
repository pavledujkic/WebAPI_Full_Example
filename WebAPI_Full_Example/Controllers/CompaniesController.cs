using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI_Full_Example.Controllers
{
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
            try
            {
                _logger.LogInfo("GetCompanies was called");

                var companies = _repository.Company.GetAllCompanies(false);
                var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

                _logger.LogInfo("Companies got from the database.");

                return Ok(companiesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetCompanies)}: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
        
