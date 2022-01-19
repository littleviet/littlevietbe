using System.Security.Claims;
using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : BaseController
{
    private readonly IProductDomain _productDomain;
    public ProductController(IProductDomain productDomain)
    {
        _productDomain = productDomain ?? throw new ArgumentNullException(nameof(productDomain));
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("")]
    public async Task<IActionResult> Create(CreateProductViewModel createProductViewModel)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productDomain.Create(Guid.Parse(userId), createProductViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(UpdateProductViewModel updateProductViewModel)
    {
        try
        {
            var result = await _productDomain.Update(updateProductViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var result = await _productDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetListProducts([FromQuery] BaseListQueryParameters parameters)
    {
        try
        {
            var result = await _productDomain.GetListProducts(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] BaseSearchParameters parameters)
    {
        try
        {
            var result = await _productDomain.Search(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetProductDetails(Guid id)
    {
        try
        {
            var result = await _productDomain.GetProductById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [HttpGet("menu")]
    public IActionResult GetProductsMenu()
    {
        try
        {
            var result = _productDomain.GetProductsMenu();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

