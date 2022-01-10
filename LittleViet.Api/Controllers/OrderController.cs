using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/order")]
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
    public async Task<IActionResult> DeactivateAccount(Guid id)
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

