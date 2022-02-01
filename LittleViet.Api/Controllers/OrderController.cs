using LittleViet.Data.Domains;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LittleViet.Api.Utilities;
using LittleViet.Data.Domains.Order;
using LittleViet.Data.Models;
using LittleViet.Infrastructure.Utilities;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : Controller
{
    private readonly IOrderDomain _orderDomain;
    public OrderController(IOrderDomain orderDomain)
    {
        _orderDomain = orderDomain ?? throw new ArgumentNullException(nameof(orderDomain));
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Create(CreateOrderViewModel createOrderViewModel)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            createOrderViewModel.AccountId = userId;
            var result = await _orderDomain.Create(createOrderViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    [NonAction]
    public async Task<IActionResult> Update(UpdateOrderViewModel updateOrderViewModel)
    {
        try
        {
            var result = await _orderDomain.Update(updateOrderViewModel);
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
            var result = await _orderDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetListOrders([FromQuery] BaseListQueryParameters parameters)
    {
        try
        {
            var result = await _orderDomain.GetListOrders(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchOrders([FromQuery] BaseSearchParameters parameters)
    {
        try
        {
            var result = await _orderDomain.Search(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetOrderDetails(Guid id)
    {
        try
        {
            var result = await _orderDomain.GetOrderById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

