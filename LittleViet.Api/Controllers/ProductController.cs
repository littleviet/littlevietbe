using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using LittleViet.Api.Utilities;
using LittleViet.Data.Domains.Product;
using LittleViet.Data.Models;

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
    public async Task<IActionResult> Create([FromForm] CreateProductViewModel createProductViewModel)
    {
        try
        {
            var result = await _productDomain.Create(createProductViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromForm] UpdateProductViewModel updateProductViewModel)
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
    public IActionResult GetProductDetails(Guid id)
    {
        try
        {
            var result = _productDomain.GetProductById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

