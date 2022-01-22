using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductTypeController : BaseController
{
    private readonly IProductTypeDomain _productTypeDomain;
    public ProductTypeController(IProductTypeDomain productTypeDomain)
    {
        _productTypeDomain = productTypeDomain ?? throw new ArgumentNullException(nameof(productTypeDomain));
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("")]
    public async Task<IActionResult> Create(CreateProductTypeViewModel productTypeViewModel)
    {
        try
        {
            var result = await _productTypeDomain.Create(productTypeViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(UpdateProductTypeViewModel productTypeVm)
    {
        try
        {
            var result = await _productTypeDomain.Update(productTypeVm);
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
            var result = await _productTypeDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    // [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetListProductTypes([FromQuery] BaseListQueryParameters parameters)
    {
        try
        {
            var result = await _productTypeDomain.GetListProductTypes(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchProductTypes([FromQuery] BaseSearchParameters parameters)
    {
        try
        {
            var result = await _productTypeDomain.Search(parameters);
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
            var result = await _productTypeDomain.GetProductTypeById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}
