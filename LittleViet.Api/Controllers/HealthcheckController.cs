using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers
{
    [Route("healthcheck")]
    [ApiController]
    public class HealthcheckController : BaseController
    {
        [HttpGet()]
        public IActionResult Get()
        {
            return Ok("LittleViet API is working OK!");
        }
    }
}
