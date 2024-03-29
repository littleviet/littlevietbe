﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LittleViet.Infrastructure.Security.JWT;

public static class AddAuthenticationExtensions
{
    public static IServiceCollection AddAppJwtAuthentication(this IServiceCollection serviceCollection, IConfiguration configurationManager)
    {
        var appSettings = configurationManager.GetRequiredSection(AppSettings.ConfigSection).Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.JwtSecret);
        serviceCollection.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return serviceCollection;
    }
}