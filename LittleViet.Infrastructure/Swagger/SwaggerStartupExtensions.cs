using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace LittleViet.Infrastructure.Swagger;

public static class SwaggerStartupExtensions
{
    public static IServiceCollection AddApplicationSwagger(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSwaggerGen(c =>
        {
            c.OperationFilter<CustomFromBodyOperationFilter>();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new[]
                    {
                        "Bearer"
                    }
                }
            });
            c.DescribeAllParametersInCamelCase();
            c.UseInlineDefinitionsForEnums();
        });
}