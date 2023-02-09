using LittleViet.Domain.Domains.Coupon;
using LittleViet.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponTypeController : BaseController
{

    private readonly ICouponTypeDomain _couponTypeDomain;
    public CouponTypeController(ICouponTypeDomain couponTypeDomain)
    {
        _couponTypeDomain = couponTypeDomain ?? throw new ArgumentNullException(nameof(couponTypeDomain));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCouponTypeViewModel createCouponTypeViewModel)
    {
        try
        {
            var result = await _couponTypeDomain.CreateCouponType(createCouponTypeViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCouponTypes()
    {
        try
        {
            var result = await _couponTypeDomain.GetCouponTypes();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

