using System.Security.Claims;
using LittleViet.Api.Utilities;
using LittleViet.Data.Domains;
using LittleViet.Data.Domains.Account;
using LittleViet.Data.Models;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LittleViet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : BaseController
{
    private readonly IAccountDomain _accountDomain;
    public AccountController(IAccountDomain accountDomain)
    {
        _accountDomain = accountDomain ?? throw new ArgumentNullException(nameof(accountDomain));
    }

    [HttpPost("login")]
    public IActionResult Login(LoginViewModel loginViewModel)
    {
        try
        {
            Log.Information("Begin login with {userEmail}", loginViewModel.Email);
            var result = _accountDomain.Login(loginViewModel.Email, loginViewModel.Password);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateAccountViewModel createAccountViewModel)
    {
        try
        {
            var result = await _accountDomain.Register(createAccountViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountViewModel createAccountViewModel)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountDomain.Create(Guid.Parse(userId), createAccountViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(UpdateAccountViewModel updateAccountViewModel)
    {
        try
        {
            var result = await _accountDomain.Update(updateAccountViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}/reset-password")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel updatePasswordViewModel)
    {
        try
        {
            var result = await _accountDomain.UpdatePassword(updatePasswordViewModel);
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
            var result = await _accountDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetListAccounts([FromQuery] BaseListQueryParameters parameters)
    {
        try
        {
            var result = await _accountDomain.GetListAccounts(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchAccounts([FromQuery] BaseSearchParameters parameters)
    {
        try
        {
            var result = await _accountDomain.Search(parameters);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetAccountDetails(Guid id)
    {
        try
        {
            var result = await _accountDomain.GetAccountById(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

}

