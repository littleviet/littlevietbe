using LittleViet.Infrastructure.Stripe;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Infrastructure.Configurations;

public static class StartupConfigurationExtensions
{
    public static ConfigureHostBuilder AddConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            const string configurationsDirectory = "Configurations";
            var env = context.HostingEnvironment.EnvironmentName;
            config.AddJsonFile($"{configurationsDirectory}/application.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/application.{env}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/database.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/database.{env}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/logging.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/logging.{env}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/stripe.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/stripe.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
        
        return host;
    }
    
    public static IServiceCollection AddConfigurationBinding(this IServiceCollection serviceCollection, IConfiguration configurationManager)
    {
        serviceCollection
            .Configure<StripeSettings>(configurationManager.GetSection(StripeSettings.ConfigSection));        
        
        return serviceCollection;
    }
}