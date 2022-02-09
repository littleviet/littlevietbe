using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using LittleViet.Data;
using LittleViet.Data.Domains;
using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;
using LittleViet.Infrastructure.Configurations;
using LittleViet.Infrastructure.Logging;
using LittleViet.Infrastructure.Middleware;
using LittleViet.Infrastructure.Security.JWT;
using LittleViet.Infrastructure.Stripe;
using LittleViet.Infrastructure.Swagger;
using LittleViet.Infrastructure.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore.Diagnostics;
using LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    Log.Information("Starting up");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    builder.Services
        .AddConfigurationBinding(builder.Configuration)
        .AddAppLoggingAndTelemetry(builder.Configuration);
    
    builder.Host
        .UseAppSerilog();

    builder.Services
        .AddDbContext<LittleVietContext>(options =>
            options.UseLazyLoadingProxies()
                .UseNpgsql(builder.Configuration.GetConnectionString("LittleVietContext"))
                .ConfigureWarnings(warn => warn.Ignore(CoreEventId.DetachedLazyLoadingWarning)))
        .AddMvc(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            // options.ModelBinderProviders.InsertBodyAndRouteBinding();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddFluentValidation(fv
            => fv.RegisterValidatorsFromAssemblyContaining<BaseDomain>(lifetime: ServiceLifetime.Singleton));

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer()
        .AddApplicationSwagger()
        .AddHttpContextAccessor()
        .AddAppJwtAuthentication(builder.Configuration)
        .ConfigureLegacy()
        .ConfigureStripe(builder.Configuration);

    var app = builder.Build();

    app.UseAppMiddlewares();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
    throw;
}
finally
{
    Log.Information("Shutting down complete");
    Log.CloseAndFlush();
}