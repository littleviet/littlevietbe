using Microsoft.AspNetCore.Builder;

namespace LittleViet.Infrastructure.Middleware;

public static class StartupMiddlewareExtensions
{
    public static WebApplication AddAppMiddleware(this WebApplication webApplication)
    {
        webApplication.UseSwagger()
            .UseSwaggerUI();

        webApplication.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        webApplication.UseAuthentication();

        webApplication.UseAuthorization();

        webApplication.UseHttpsRedirection();

        webApplication.MapControllers();

        return webApplication;
    }
}