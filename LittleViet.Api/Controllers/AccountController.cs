using LittleViet.Data.Domains;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleViet.Data.ServiceHelper;

namespace LittleViet.Api.Controllers;

    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        private IAccountDomain _accountDomain;
        public AccountController(IAccountDomain accountDomain)
        {
            _accountDomain = accountDomain;
        }
        [HttpPost("login")]
        public IActionResult Login(LoginVM accountVM)
        {
            try
            {
                var result = _accountDomain.Login(accountVM.Email, accountVM.Password);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
        [HttpPost("")]
        public IActionResult Create(CreateAccountVM accountVM)
        {
            try
            {
                var result = _accountDomain.Create(accountVM);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
        [HttpPut("id")]
        public IActionResult Update(Guid id, UpdateAccountVM accountVM)
        {
            try
            {
                accountVM.Id = id;
                var result = _accountDomain.Update(accountVM);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
        [HttpPut("password/id")]
        public IActionResult UpdatePassword(Guid id, UpdatePasswordVM accountVM)
        {
            try
            {
                accountVM.Id = id;
                var result = _accountDomain.UpdatePassword(accountVM);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
        [HttpDelete("id")]
        public IActionResult DeactiveAccount(Guid id)
        {
            try
            {
                var result = _accountDomain.Deactivate(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }
    }

