using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CompanyEmployees.Presentation.ActionFilters;

public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var param = context.ActionArguments.SingleOrDefault(x =>
            x.Value!.ToString()!.Contains("Dto")).Value;

        if (param == null)
        {
            context.Result = new BadRequestObjectResult($"Object is null. Controller: {context.RouteData.Values["controller"]}, action: {context.RouteData.Values["action"]}");

            return;
        }

        if (context.ModelState.IsValid)
            return;

        context.Result = new UnprocessableEntityObjectResult(context.ModelState);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}