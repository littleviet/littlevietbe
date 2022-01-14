﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace LittleViet.Infrastructure.Configurations;

public static class StartupConfigurationExtensions
{
    public static ConfigureHostBuilder AddConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            const string configurationsDirectory = "Configurations";
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/stripe.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
        return host;
    }
}