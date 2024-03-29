﻿using System.Security.Claims;
using LittleViet.Api.Utilities;
using LittleViet.Domain.Domains.Coupon;
using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponController : BaseController
{

    private readonly ICouponDomain _couponDomain;
    public CouponController(ICouponDomain couponDomain)
    {
        _couponDomain = couponDomain ?? throw new ArgumentNullException(nameof(couponDomain));
    }

    [HttpPost("")]
    public async Task<IActionResult> Create(CreateCouponViewModel createCouponViewModel)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            createCouponViewModel.AccountId = Guid.TryParse(userId, out var parsedId) ? parsedId : null;
            var result = await _couponDomain.CreateCoupon(createCouponViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBodyAndRoute] UpdateCouponViewModel updateCouponViewModel)
    {
        try
        {
            var result = await _couponDomain.UpdateCoupon(updateCouponViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }


    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(UpdateCouponStatusViewModel updateCouponStatusViewModel)
    {
        try
        {
            var result = await _couponDomain.UpdateStatus(updateCouponStatusViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var result = await _couponDomain.DeactivateCoupon(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetListCoupons([FromQuery] BaseListQueryParameters parameters)
    {
        try
        {
            var result = await _couponDomain.GetListCoupons(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchCoupons([FromQuery] BaseSearchParameters parameters)
    {
        try
        {
            var result = await _couponDomain.Search(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCouponDetails(Guid id)
    {
        try
        {
            var result = await _couponDomain.GetCouponById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

