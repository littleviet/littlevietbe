using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController : Controller
{
    private IProductDomain _productDomain;
    public ProductController(IProductDomain productDomain)
    {
        _productDomain = productDomain;
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("")]
    public IActionResult Create(CreateProductViewModel productVM)
    {
        try
        {
            var result = _productDomain.Create(productVM);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, UpdateProductViewModel productVM)
    {
        try
        {
            productVM.Id = id;
            var result = _productDomain.Update(productVM);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpDelete("{id}")]
    public IActionResult DeactiveAccount(Guid id)
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

    [HttpGet("landing-page/products")]
    public IActionResult GetProductsForLP()
    {
        try
        {
            var result = _productDomain.GetActivesForLP();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

