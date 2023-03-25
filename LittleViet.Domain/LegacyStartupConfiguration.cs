using LittleViet.Domain.Domains;
using LittleViet.Domain.Models;
using LittleViet.Domain.Repositories;
using LittleViet.Domain.Services.HealthChecks;
using LittleViet.Infrastructure.Azure.AzureBlobStorage;
using LittleViet.Infrastructure.DateTime;
using LittleViet.Infrastructure.Email;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Service;
using LittleViet.Infrastructure.Stripe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace LittleViet.Domain;

public static class LegacyStartupConfiguration
{
    public static IServiceCollection AddLegacyDi(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(LegacyStartupConfiguration));
        services.AddAppDi();
        return services;
    }

    private static IServiceCollection AddAppDi(this IServiceCollection services) =>
        services.AddDataAccess()
            .AddDomainsAndRepositories()
            .AddAppStripeServices()
            .AddAppAzureBlobStorage()
            .AddAppEmailServices()
            .AddAppDateTimeServices()
            .AddAppHealthChecks();

    private static IServiceCollection AddDomainsAndRepositories(this IServiceCollection services) =>
        services.Scan(
            scan =>
                scan.FromAssembliesOf(typeof(BaseDomain))
                    .AddClasses(x => x.Where(type =>
                        type.Name.EndsWith("Repository") ||
                        type.Name.EndsWith("Domain")
                    ))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

    private static IServiceCollection AddDataAccess(this IServiceCollection services) =>
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<DbContext, LittleVietContext>();
}