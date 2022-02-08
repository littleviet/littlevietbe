using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LittleViet.Infrastructure.Swagger;

// Converts any custom BindingSource that accepts Body into RequestBody parameters in Swagger
// See: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1002#issuecomment-760002223
public class CustomFromBodyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiBodyParameter =
            context.ApiDescription.ParameterDescriptions.FirstOrDefault(p =>
                p.Source.CanAcceptDataFrom(BindingSource.Body));

        if (apiBodyParameter == null) return;

        var swaggerQueryParameter = operation.Parameters
            .FirstOrDefault(p => p.Name == apiBodyParameter.Name && p.In == ParameterLocation.Query);

        if (swaggerQueryParameter == null) return;

        operation.Parameters.Remove(swaggerQueryParameter);
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = {["application/json"] = new OpenApiMediaType {Schema = swaggerQueryParameter.Schema}}
        };
    }
}