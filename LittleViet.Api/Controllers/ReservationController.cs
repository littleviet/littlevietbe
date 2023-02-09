using System.Security.Claims;
using LittleViet.Api.Utilities;
using LittleViet.Domain.Domains.Reservations;
using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : Controller
{
    private readonly IReservationDomain _reservationDomain;

    public ReservationController(IReservationDomain reservationDomain)
    {
        _reservationDomain = reservationDomain;
    }

    [HttpPost("")]
    public async Task<IActionResult> Create(CreateReservationViewModel reservationViewModel)
    {
        try
        {
            var result = await _reservationDomain.Create(reservationViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.MANAGER, Role.ADMIN)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBodyAndRoute] UpdateReservationViewModel reservationViewModel)
    {
        try
        {
            var parseSuccessful = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
            reservationViewModel.AccountId = parseSuccessful ? userId : throw new Exception($"Id not parsable");
            var result = await _reservationDomain.Update(reservationViewModel);
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
            var result = await _reservationDomain.Deactivate(id);
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
            var result = await _reservationDomain.GetListReservations(parameters);
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
            var result = await _reservationDomain.GetReservationById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

