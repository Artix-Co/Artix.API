namespace Artix.API.Endpoints.Middlewares;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Core.Contract.Primitives.Infra.Audit;

public sealed class AdminAuditMiddleware(RequestDelegate next, ILogger<AdminAuditMiddleware> logger, IAdminAuditWriter auditWriter)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (!context.Request.Path.StartsWithSegments("/api/admin") || context.User.Identity?.IsAuthenticated != true)
            return;

        var userId = long.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedUserId) ? parsedUserId : (long?)null;
        var roles = string.Join(',', context.User.FindAll(ClaimTypes.Role).Select(x => x.Value));
        await auditWriter.WriteAsync(userId, context.Request.Method, context.Request.Path, context.Response.StatusCode, roles,
            context.Connection.RemoteIpAddress?.ToString(), context.RequestAborted);

        logger.LogInformation(
            "Admin audit: Method={Method}, Path={Path}, StatusCode={StatusCode}, UserId={UserId}, Roles={Roles}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            userId?.ToString() ?? "unknown", roles);
    }
}
