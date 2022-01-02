using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/product-type")]
[ApiController]
public class ProductTypeController : BaseController
{
    private IProductTypeDomain _productTypeDomain;
    public ProductTypeController(IProductTypeDomain productTypeDomain)
    {
        _productTypeDomain = productTypeDomain;
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("")]
    public IActionResult Create(CreateProductTypeViewModel productTypeVM)
    {
        try
        {
            var result = _productTypeDomain.Create(productTypeVM);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, UpdateProductTypeViewModel productTypeVM)
    {
        try
        {
            productTypeVM.Id = id;
            var result = _productTypeDomain.Update(productTypeVM);
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
            var result = _productTypeDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public IActionResult GetListProductTypes()
    {
        try
        {
            var result = _productTypeDomain.GetListProductType();
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
            var result = _productTypeDomain.GetProductsGroupByType();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

}
