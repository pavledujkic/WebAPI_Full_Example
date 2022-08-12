using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CompanyEmployees.ActionFilters;

public class ValidateCompanyExistsAttribute : IAsyncActionFilter
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    public ValidateCompanyExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var isCompaniesController = context.Controller is Controllers.CompaniesController;
        var trackChanges = method.Equals("PUT");
        Guid id;

        if (isCompaniesController)
        {
            id = (Guid)context.ActionArguments["id"]!;
        }
        else
        {
            id = (Guid)context.ActionArguments["companyId"]!;
        }

        Company? company = await _repository.Company.GetCompanyAsync(id, trackChanges);

        if (company == null)
        {
            _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

            context.Result = new NotFoundResult();
        }
        else if (isCompaniesController || !method.Equals("GET"))
        {
            context.HttpContext.Items.Add("company", company);
            await next();
        }
        else
        {
            await next();
        }
    }
}