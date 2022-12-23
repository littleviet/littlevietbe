using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using LittleViet.Domain;
using LittleViet.Domain.Domains;
using LittleViet.Domain.Models;
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

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    Log.Information("Starting up");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    if (environment == Environments.Development)
        builder.Configuration.AddUserSecrets<Program>();
    
    builder.Services
        .AddConfigurationBinding(builder.Configuration)
        .AddAppLoggingAndTelemetry(builder.Configuration);
    
    builder.Host
        .UseAppSerilog();

    builder.Services //TODO: move data to separate project
        .AddDbContext<LittleVietContext>(options =>
            options.UseLazyLoadingProxies()
                .UseNpgsql(builder.Configuration.GetConnectionString("LittleVietContext"))
                .ConfigureWarnings(warn => warn.Ignore(CoreEventId.DetachedLazyLoadingWarning)))
        .AddMvc(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            options.ModelBinderProviders.InsertBodyAndRouteBinding();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddFluentValidation(fv
            => fv.RegisterValidatorsFromAssemblyContaining<BaseDomain>(lifetime: ServiceLifetime.Transient));

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer()
        .AddApplicationSwagger()
        .AddHttpContextAccessor()
        .AddAppJwtAuthentication(builder.Configuration)
        .AddLegacyDi();

    var app = builder.Build();
    
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);//TODO: fix or workaround this
    using (var scope = app.Services.CreateScope())
    {
        using (var context = scope.ServiceProvider.GetService<LittleVietContext>())
        context.Database.Migrate();
    }

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