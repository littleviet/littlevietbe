using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Infrastructure.Security.XSRF;

public static class UseAppXsrfExtensions
{
    public static IServiceCollection ConfigureXsrf(this IServiceCollection serviceCollection) 
        => serviceCollection.AddAntiforgery(options =>
        {
            // options.Cookie.Name = 
            // options.HeaderName = "X-XSRF-TOKEN";
        });
}