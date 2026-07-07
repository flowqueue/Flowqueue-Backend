using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Flowqueue_Backend.shared.Infrastructure.Interfaces.ASP.Configuration;

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowsAnonymous = context.MethodInfo
            .GetCustomAttributes(inherit: true)
            .OfType<AllowAnonymousAttribute>()
            .Any()
            || context.MethodInfo.DeclaringType?
                .GetCustomAttributes(inherit: true)
                .OfType<AllowAnonymousAttribute>()
                .Any() == true;

        if (allowsAnonymous)
        {
            operation.Security = new List<OpenApiSecurityRequirement>();
            return;
        }

        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
    }
}
