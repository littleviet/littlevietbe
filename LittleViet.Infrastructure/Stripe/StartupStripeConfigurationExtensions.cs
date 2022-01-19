using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace LittleViet.Infrastructure.Stripe;

public static class StartupStripeConfigurationExtensions
{
    public static IServiceCollection ConfigureStripe(this IServiceCollection serviceCollection, IConfiguration configurationManager)
    {
        var stripeSettings = configurationManager.GetSection(StripeSettings.ConfigSection).Get<StripeSettings>();
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;
        return serviceCollection;
    }
}