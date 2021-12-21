using LittleVietApi.Controllers;
using LittleVietData.Domains;
using LittleVietData.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleVietBE.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : BaseController
    {
        [HttpGet("Login")]
        public IActionResult Login(string email, string password)
        {
            try
            {
                var accD = Service<IAccountDomain>();
                var result = accD.Login(email, password);
                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, new ResponseViewModel { Message = e.Message, Success = false });
            }
        }
    }
}
