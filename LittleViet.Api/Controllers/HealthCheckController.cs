using LittleViet.Data.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/healthcheck")]
[ApiController]
public class HealthcheckController : BaseController
{
    private IProductTypeDomain _productTypeDomain;
    public HealthcheckController(IProductTypeDomain productTypeDomain)
    {
        _productTypeDomain = productTypeDomain;
    }

    [HttpGet("api-check")]
    public IActionResult ApiCheck()
    {
        try
        {
            return Ok("LittleViet API is working okay!");
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [HttpGet("db-check")]
    public IActionResult DBCheck()
    {
        try
        {
            _productTypeDomain.GetProductsGroupByType();
            return Ok("Working");
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}

