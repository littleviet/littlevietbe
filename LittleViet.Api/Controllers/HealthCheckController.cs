using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers
{
    
    [Route("api/healthCheck")]
    [ApiController]
    public class HealthCheckController : BaseController
    {
        [Authorize]
        [HttpGet("apicheck")]
        public IActionResult ApiCheck()
        {
            try
            {
                
                return Ok("Woking");
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
        [HttpGet("dbcheck")]
        public IActionResult DBCheck()
        {
            try
            {

                return Ok("Working");
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}
