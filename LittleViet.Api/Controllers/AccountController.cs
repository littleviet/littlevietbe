using LittleViet.Api.Controllers;
using LittleViet.Data.Domains;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("Login")]
        public IActionResult Login(string email, string password)
        {
            try
            {
                var result = _accountDomain.Login(email, password);
                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
            }
        }
    }
}
