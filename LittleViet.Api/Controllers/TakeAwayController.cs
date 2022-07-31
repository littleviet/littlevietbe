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
    public async Task<ResponseViewModel<List<GetListProductViewModel>>> GetProductsForTakeAway() => await _landingPageDomain.GetMenuForTakeAway();
}
