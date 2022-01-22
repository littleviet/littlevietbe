using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace LittleViet.Infrastructure.Security.XSRF;

public class XsrfMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAntiforgery _antiforgery;

    public XsrfMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // string path = context.Request.Path.Value;
        //
        // if (
        //     string.Equals(path, "/", StringComparison.OrdinalIgnoreCase))
        {
            var tokens = _antiforgery.GetAndStoreTokens(context);
            context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, 
                new CookieOptions() { HttpOnly = false });
        }

        await _next(context);
    }
}