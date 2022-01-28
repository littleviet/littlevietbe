using LittleViet.Data.Domains;
using LittleViet.Data.Domains.TakeAway;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TakeAwayController : BaseController
{
    private readonly ITakeAwayDomain _landingPageDomain;
    public TakeAwayController(ITakeAwayDomain takeAwayDomain)
    {
        _landingPageDomain = takeAwayDomain ?? throw new ArgumentNullException(nameof(takeAwayDomain));
    }

    [HttpGet("menu")]
    public async Task<IActionResult> GetProductsForTakeAway()
    {
        try
        {
            var result = await _landingPageDomain.GetMenuForTakeAway();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

}
