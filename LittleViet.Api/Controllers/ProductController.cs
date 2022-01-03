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
    public IActionResult Create(CreateProductViewModel productVm)
    {
        try
        {
            var result = _productDomain.Create(productVm);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateProductViewModel productVm)
    {
        try
        {
            productVm.Id = id;
            var result = _productDomain.Update(productVm);
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
}

