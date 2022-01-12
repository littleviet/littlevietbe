using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
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

    [Authorize]
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
    public async Task<IActionResult> Update(UpdateReservationViewModel reservationViewModel)
    {
        try
        {
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

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER, Role.UNAUTHORIZED, Role.AUTHORIZED)]
    [HttpGet]
    public async Task<IActionResult> GetListReservations([FromQuery] BaseListQueryParameters parameters)
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

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER, Role.UNAUTHORIZED, Role.AUTHORIZED)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchReservations([FromQuery] BaseSearchParameters parameters)
    {
        try
        {
            var result = await _resevationDomain.Search(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetProductTypeDetails(Guid id)
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

