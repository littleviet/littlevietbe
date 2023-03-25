using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LittleViet.Domain.Services.HealthChecks;

public static class HealthCheckServicesExtensions
{
    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        => services.AddHealthChecks()
            .AddCheck<DatabaseCheck>(
                "Database",
                failureStatus: HealthStatus.Degraded
            )
            .AddCheck<StripeCheck>(
                "Stripe",
                failureStatus: HealthStatus.Degraded
            )
            .AddCheck("API", () => HealthCheckResult.Healthy("API is healthy"))
            .Services;
}