using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers
{
    
    [Route("api/healthcheck")]
    [ApiController]
    public class HealthcheckController : BaseController
    {
        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("LittleViet API is working okay!");
        }

        [HttpGet("api-check")]
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
        [HttpGet("db-check")]
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
