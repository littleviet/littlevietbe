using LittleViet.Data.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/healthcheck")]
[ApiController]
public class HealthcheckController : BaseController
{
    private IProductDomain _productDomain;
    public HealthcheckController(IProductDomain productDomain)
    {
        _productDomain = productDomain;
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
            _productDomain.GetActivesForLP();
            return Ok("Working");
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}

