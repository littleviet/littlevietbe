using LittleViet.Data.Models;
using LittleViet.Data.ServiceHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LittleViet.Infrastructure.Configurations;
using LittleViet.Infrastructure.Middleware;
using LittleViet.Infrastructure.Security.JWT;
using LittleViet.Infrastructure.Stripe;
using LittleViet.Infrastructure.Swagger;
using LittleViet.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddConfigurations();

builder.Services
    .AddDbContext<LittleVietContext>(options =>
        options.UseLazyLoadingProxies()
            .UseNpgsql(builder.Configuration.GetConnectionString("LittleVietContext"))
            .ConfigureWarnings(warn => warn.Ignore(CoreEventId.DetachedLazyLoadingWarning)))
    .AddMvc(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
    .AddApplicationSwagger()
    .AddHttpContextAccessor()
    .AddAppJwtAuthentication(builder.Configuration)
    .ConfigureLegacy();

var app = builder.Build();

app.UseAppMiddlewares();

app.Run();