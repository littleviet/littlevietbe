using LittleViet.Data.Models;
using System.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LittleViet.Data.ViewModels;
using System.Net;

namespace LittleViet.Api.Controllers;

[Route("api/healthcheck")]
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
            if (_context.Database.CanConnect() == true)
            {
                return Ok("Database Working");
            }
            else
            {
                return Ok(new ResponseViewModel
                {
                    Message = "Database not reachable",
                    Success = false
                });;

            }
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

