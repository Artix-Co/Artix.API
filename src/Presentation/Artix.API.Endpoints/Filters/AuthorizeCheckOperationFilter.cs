namespace Artix.API.Endpoints.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

internal sealed class AuthorizeCheckOperationFilter : IOperationFilter
{
    private static readonly Type AuthorizeType = typeof(AuthorizeAttribute);

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation == null || context == null)
            return;

        // Fast-path: check metadata instead of raw Reflection
        var hasAuthorize = 
            context.ApiDescription.ActionDescriptor.EndpointMetadata
                .Any(m => m.GetType() == AuthorizeType);

        if (!hasAuthorize)
            return;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
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
                Array.Empty<string>()
            }
        });
    }
}
