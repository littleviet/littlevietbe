using LittleViet.Infrastructure.Stripe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

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