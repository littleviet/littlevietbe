using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Stripe.Checkout;

namespace LittleViet.Infrastructure.Stripe;

public static class StartupStripeConfigurationExtensions
{
    public static IServiceCollection ConfigureStripe(this IServiceCollection serviceCollection, IConfiguration configurationManager)
    {
        var stripeSettings = configurationManager.GetRequiredSection(StripeSettings.ConfigSection).Get<StripeSettings>();
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;
        return serviceCollection;
    }

    public static IServiceCollection AddAppStripeServices(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped(_ => new ProductService())
            .AddScoped(_ => new PriceService())
            .AddScoped(_ => new SessionService())
            .AddScoped<IStripePaymentService, StripePaymentService>()
            .AddScoped<IStripeProductService, StripeProductService>()
            .AddScoped<IStripePriceService, StripePriceService>();
}