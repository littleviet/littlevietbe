using LittleViet.Data.Domains;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/landing-page")]
[ApiController]
public class LandingPageController : BaseController
{
    private readonly ILandingPageDomain _landingPageDomain;
    public LandingPageController(ILandingPageDomain landingPageDomain)
    {
        _landingPageDomain = landingPageDomain ?? throw new ArgumentNullException(nameof(landingPageDomain));
    }

    [HttpGet("products")]
    public IActionResult Get()
    {
        try
        {
            var result = _landingPageDomain.GetCatalogForLandingPage();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

}
