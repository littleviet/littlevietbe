using LittleViet.Infrastructure.Security.XSRF;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace LittleViet.Infrastructure.Middleware;

public static class StartupMiddlewareExtensions
{
    public static WebApplication UseAppMiddlewares(this WebApplication webApplication)
    {
        webApplication.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        webApplication.UseMiddleware<XsrfMiddleware>();

        webApplication.UseSerilogRequestLogging();
        
        webApplication.UseSwagger()
            .UseSwaggerUI(o =>
            {
                o.DocumentTitle = "LittleViet - API Documentation";
                o.DocExpansion(DocExpansion.None);
            });
        
        webApplication.UseAuthentication();
        
        webApplication.UseAuthorization();

        webApplication.UseHttpsRedirection();

        webApplication.MapControllers();

        return webApplication;
    }
}