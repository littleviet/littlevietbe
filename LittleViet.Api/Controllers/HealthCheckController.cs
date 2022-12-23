using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthcheckController : BaseController
{
    private readonly LittleVietContext _context;

    public HealthcheckController(LittleVietContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet("api-check")]
    public IActionResult ApiCheck()
    {
        try
        {
            return Ok("LittleViet API is working okay!");
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("db-check")]
    public IActionResult DbCheck()
    {
        try
        {
            if (_context.Database.CanConnect())
            {
                return Ok("Database Working");
            }

            return Ok(new ResponseViewModel
            {
                Message = "Database not reachable",
                Success = false
            });
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}