using LittleViet.Data.Domains;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleViet.Data.ServiceHelper;

namespace LittleViet.Api.Controllers
{
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
        public IActionResult Login(AccountVM accountVM)
        {
            try
            {
                var result = _accountDomain.Login(accountVM.Email, accountVM.Password);
                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPost("create")]
        public IActionResult Create(AccountVM accountVM)
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

        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("update")]
        public IActionResult Update(AccountVM accountVM)
        {
            try
            {
                var result = _accountDomain.Update(accountVM);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("deactive")]
        public IActionResult DeactiveAccount(AccountVM accountVM)
        {
            try
            {
                var result = _accountDomain.Deactive(accountVM.Id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }
    }
}
