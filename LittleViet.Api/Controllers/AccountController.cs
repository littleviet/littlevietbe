using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Update(Guid id, UpdateAccountViewModel updateAccountViewModel)
    {
        try
        {
            updateAccountViewModel.Id = id;
            var result = _accountDomain.Update(updateAccountViewModel);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
        }
    }

    [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
    [HttpPut("{id:guid}/reset-password")]
    public IActionResult UpdatePassword(Guid id, UpdatePasswordViewModel updatePasswordViewModel)
    {
        try
        {
            updatePasswordViewModel.Id = id;
            var result = _accountDomain.UpdatePassword(updatePasswordViewModel);
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

