using Microsoft.AspNetCore.Mvc;

namespace LittleVietApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected T Service<T>()
        {
            return HttpContext.RequestServices.GetService<T>();
        }
    }
}
