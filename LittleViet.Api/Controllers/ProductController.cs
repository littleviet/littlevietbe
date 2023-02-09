using LittleViet.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using LittleViet.Api.Utilities;
using LittleViet.Domain.Domains.Products;
using LittleViet.Domain.Models;
using LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;

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
    public async Task<BaseListResponseViewModel<GetListProductViewModel>> GetListProducts([FromQuery] GetListProductParameters parameters) => 
        await _productDomain.GetListProducts(parameters);

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
    [HttpGet("{id:guid}")]
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
    
    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("{productId:guid}/image")]
    public async Task<IActionResult> AddProductImages([FromForm] AddProductImagesViewModel addProductImagesViewModel)
    {
        try
        {
            var result = await _productDomain.AddProductImages(addProductImagesViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
    
    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpDelete("{id:guid}/image/{imageId:guid}")]
    public async Task<IActionResult> RemoveProductImageById(Guid id, Guid imageId)
    {
        try
        {
            var result = await _productDomain.DeactivateProductImage(id, imageId);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel { Message = e.Message, Success = false });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
    
    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}/image/{imageId:guid}/make-main")]
    public async Task<IActionResult> MakeMainProductImage(Guid id, Guid imageId)
    {
        try
        {
            var result = await _productDomain.MakeMainProductImage(id, imageId);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

