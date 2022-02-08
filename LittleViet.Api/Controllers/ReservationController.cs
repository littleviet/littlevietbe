using System.Security.Claims;
using LittleViet.Api.Utilities;
using LittleViet.Data.Domains;
using LittleViet.Data.Domains.Reservations;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : Controller
{
    private IReservationDomain _resevationDomain;

    public ReservationController(IReservationDomain reservationDomain)
    {
        _resevationDomain = reservationDomain;
    }

    [HttpPost("")]
    public async Task<IActionResult> Create(CreateReservationViewModel reservationViewModel)
    {
        try
        {
            var result = await _resevationDomain.Create(reservationViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.MANAGER, Role.ADMIN)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateReservationViewModel reservationViewModel)
    {
        try
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            reservationViewModel.AccountId = userId;
            reservationViewModel.Id = id;
            var result = await _resevationDomain.Update(reservationViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.MANAGER, Role.ADMIN)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var result = await _resevationDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetListReservations([FromQuery] GetListReservationParameters parameters)
    {
        try
        {
            var result = await _resevationDomain.GetListReservations(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReservationDetails(Guid id)
    {
        try
        {
            var result = await _resevationDomain.GetReservationById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

