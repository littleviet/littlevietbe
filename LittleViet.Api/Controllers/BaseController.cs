using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers
{
    public class BaseController : ControllerBase
    {

        protected T Service<T>()
        {
            return HttpContext.RequestServices.GetService<T>();
        }
    }
}
