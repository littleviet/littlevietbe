﻿using LittleViet.Api.Utilities;
using LittleViet.Domain.Domains.Coupon;
using LittleViet.Domain.Domains.Order;
using LittleViet.Domain.Domains.Reservations;
using LittleViet.Domain.Domains.Tasks;
using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : Controller
{
    private readonly ICouponDomain _couponDomain;    
    private readonly IOrderDomain _orderDomain;
    private readonly IReservationDomain _reservationDomain;
    private readonly ITaskDomain _taskDomain;

    public TaskController(ICouponDomain couponDomain, IOrderDomain orderDomain, IReservationDomain reservationDomain, ITaskDomain taskDomain)
    {
        _couponDomain = couponDomain ?? throw new ArgumentNullException(nameof(couponDomain));
        _orderDomain = orderDomain ?? throw new ArgumentNullException(nameof(orderDomain));
        _reservationDomain = reservationDomain ?? throw new ArgumentNullException(nameof(reservationDomain));
        _taskDomain = taskDomain;
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
    
    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("initialize")]
    public async Task<IActionResult> Initialize()
    {
        try
        {
            return Ok(await _taskDomain.Initialize());
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
    
    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("pickup-takeaway")]
    public async Task<IActionResult> PickUpTakeAwayOrder(Guid orderId)
    {
        try
        {
            var result = await _orderDomain.PickupTakeAwayOrder(orderId);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
    
    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("check-in-reservation")]
    public async Task<IActionResult> CheckinReservation(Guid reservationId)
    {
        try
        {
            var result = await _reservationDomain.CheckInReservation(reservationId);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

