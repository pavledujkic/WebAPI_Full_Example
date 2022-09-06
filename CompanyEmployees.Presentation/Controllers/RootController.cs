using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api")]
[ApiController]
public class RootController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;

    public RootController(LinkGenerator linkGenerator) =>
        _linkGenerator = linkGenerator;

    [HttpGet(Name = "GetRoot")]
    public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
    {
        if (!mediaType.Contains("application/vnd.pd.apiroot"))
            return NoContent();

        var list = new List<Link>(3)
        {
            new()
            {
                Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}),
                Rel = "self",
                Method = "GET"
            },
            new()
            {
                Href = _linkGenerator.GetUriByName(HttpContext, "GetCompanies", new {}),
                Rel = "companies",
                Method = "GET"
            },
            new()
            {
                Href = _linkGenerator.GetUriByName(HttpContext, "CreateCompany", new {}),
                Rel = "create_company",
                Method = "POST"
            }
        };

        return Ok(list);
    }
}