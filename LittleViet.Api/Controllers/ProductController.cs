using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController : Controller
{
    private readonly IProductDomain _productDomain;
    public ProductController(IProductDomain productDomain)
    {
        _productDomain = productDomain;
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("")]
    public IActionResult Create(CreateProductViewModel createProductViewModel)
    {
        try
        {
            var result = _productDomain.Create(createProductViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateProductViewModel updateProductViewModel)
    {
        try
        {
            updateProductViewModel.Id = id;
            var result = _productDomain.Update(updateProductViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpDelete("{id:guid}")]
    public IActionResult DeactivateAccount(Guid id)
    {
        try
        {
            var result = _productDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

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
}

