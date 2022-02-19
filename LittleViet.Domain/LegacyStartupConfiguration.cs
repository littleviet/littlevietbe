using LittleViet.Data.Domains;
using LittleViet.Data.Models;
using LittleViet.Data.Repositories;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Interface;
using LittleViet.Infrastructure.Azure.AzureBlobStorage.Service;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Service;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Stripe;
using Stripe.Checkout;

namespace LittleViet.Data;

public static class LegacyStartupConfiguration
{
    private static IServiceCollection ConfigureIoC(this IServiceCollection services)
    {
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<DbContext, LittleVietContext>();

        services.Scan(scan =>
            scan.FromAssembliesOf(typeof(BaseDomain))
                .AddClasses(x => x.Where(type => 
                    type.Name.EndsWith("Repository") ||
                    type.Name.EndsWith("Domain")
                    ))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddScoped(_ => new ProductService())
            .AddScoped(_ => new PriceService())
            .AddScoped(_ => new SessionService())
            .AddScoped<IStripePaymentService, StripePaymentService>()
            .AddScoped<IStripeProductService, StripeProductService>()
            .AddScoped<IStripePriceService, StripePriceService>()
            .AddScoped<IBlobProductImageService, BlobProductImageService>()
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<ITemplateService, TemplateService>();

        return services;
    }

    public static IServiceCollection ConfigureLegacy(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(LegacyStartupConfiguration));
        services.ConfigureIoC();
        return services;
    }
}