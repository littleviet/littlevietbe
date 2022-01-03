using LittleViet.Data.Domains;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using LittleViet.Data.ServiceHelper;

namespace LittleViet.Api.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : BaseController
{
    private readonly IAccountDomain _accountDomain;
    public AccountController(IAccountDomain accountDomain)
    {
        _accountDomain = accountDomain;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginViewModel loginViewModel)
    {
        try
        {
            var result = _accountDomain.Login(loginViewModel.Email, loginViewModel.Password);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPost]
    public IActionResult Create(CreateAccountViewModel createAccountViewModel)
    {
        try
        {
            var result = _accountDomain.Create(createAccountViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateAccountViewModel accountVm)
    {
        try
        {
            accountVm.Id = id;
            var result = _accountDomain.Update(accountVm);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}/reset-password")]
    public IActionResult UpdatePassword(Guid id, UpdatePasswordViewModel accountVm)
    {
        try
        {
            accountVm.Id = id;
            var result = _accountDomain.UpdatePassword(accountVm);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpDelete("{id:guid}")]
    public IActionResult DeactivateAccount(Guid id)
    {
        try
        {
            var result = _accountDomain.Deactivate(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }
}

