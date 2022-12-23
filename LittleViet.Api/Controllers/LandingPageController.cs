using LittleViet.Domain.Domains.LandingPage;
using LittleViet.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LandingPageController : BaseController
{
    private readonly ILandingPageDomain _landingPageDomain;
    public LandingPageController(ILandingPageDomain landingPageDomain)
    {
        _landingPageDomain = landingPageDomain ?? throw new ArgumentNullException(nameof(landingPageDomain));
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var result = await _landingPageDomain.GetCatalogForLandingPage();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

}
