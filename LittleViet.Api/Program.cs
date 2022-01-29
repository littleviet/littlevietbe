using FluentValidation.AspNetCore;
using LittleViet.Data;
using LittleViet.Data.Domains;
using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;
using LittleViet.Infrastructure.Configurations;
using LittleViet.Infrastructure.Middleware;
using LittleViet.Infrastructure.Security.JWT;
using LittleViet.Infrastructure.Swagger;
using LittleViet.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddConfigurations();

builder.Services.AddConfigurationBinding(builder.Configuration);

builder.Services
    .AddDbContext<LittleVietContext>(options =>
        options.UseLazyLoadingProxies()
            .UseNpgsql(builder.Configuration.GetConnectionString("LittleVietContext"))
            .ConfigureWarnings(warn => warn.Ignore(CoreEventId.DetachedLazyLoadingWarning)))
    .AddMvc(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        })
    .AddFluentValidation(fv 
        => fv.RegisterValidatorsFromAssemblyContaining<BaseDomain>(lifetime: ServiceLifetime.Singleton));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
    .AddApplicationSwagger()
    .AddHttpContextAccessor()
    .AddAppJwtAuthentication(builder.Configuration)
    .ConfigureLegacy();

var app = builder.Build();

app.UseAppMiddlewares();

app.Run();