using LittleViet.Api.Utilities;
using LittleViet.Data.Domains.Coupon;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : Controller
{
    private readonly ICouponDomain _couponDomain;
    public TaskController(ICouponDomain couponDomain)
    {
        _couponDomain = couponDomain ?? throw new ArgumentNullException(nameof(couponDomain));
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("use-coupon")]
    public async Task<IActionResult> UseCoupon(UseCouponViewModel useCouponViewModel)
    {
        try
        {
            var result = await _couponDomain.RedeemCoupon(useCouponViewModel.couponCode, useCouponViewModel.usage);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

