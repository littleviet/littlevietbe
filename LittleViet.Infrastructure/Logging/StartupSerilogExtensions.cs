using LittleViet.Infrastructure.Azure;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LittleViet.Infrastructure.Logging;

public static class AppSerilogExtensions
{
    public static ConfigureHostBuilder UseAppSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((ctx, services, configuration) =>
        {
            var a = services.GetRequiredService<TelemetryConfiguration>();
            
            configuration.WriteTo.Console()
                .WriteTo.ApplicationInsights(
                    services.GetRequiredService<TelemetryConfiguration>(),
                    TelemetryConverter.Traces)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .ReadFrom.Configuration(ctx.Configuration);
        });

        return host;
    }
    
    public static IServiceCollection AddAppLoggingAndTelemetry(this IServiceCollection services, IConfiguration config)
    {
        var azureSettings = config.GetSection(AzureSettings.ConfigSection).Get<AzureSettings>();
        return services.AddApplicationInsightsTelemetry(azureSettings.AppInsightsKey);
    }
}