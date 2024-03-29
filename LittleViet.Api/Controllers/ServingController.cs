﻿using LittleViet.Api.Utilities;
using LittleViet.Domain.Domains.Serving;
using LittleViet.Domain.Models;
using LittleViet.Domain.ViewModels;
using LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServingController : Controller
{
    private readonly IServingDomain _servingDomain;
    public ServingController(IServingDomain servingDomain)
    {
        _servingDomain = servingDomain ?? throw new ArgumentNullException(nameof(servingDomain));
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost("")]
    public async Task<IActionResult> Create(CreateServingViewModel createServingViewModel)
    {
        try
        {
            var result = await _servingDomain.Create(createServingViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBodyAndRoute] UpdateServingViewModel updateServingViewModel)
    {
        try
        {
            var result = await _servingDomain.Update(updateServingViewModel);
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
            var result = await _servingDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetListServings([FromQuery] GetListServingParameters parameters)
    {
        try
        {
            var result = await _servingDomain.GetListServing(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchServings([FromQuery] BaseSearchParameters parameters)
    {
        try
        {
            var result = await _servingDomain.Search(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetServingDetails(Guid id)
    {
        try
        {
            var result = await _servingDomain.GetServingById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

