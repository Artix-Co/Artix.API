namespace Artix.API.Endpoints.Middlewares;

using System.Security.Claims;

public sealed class AdminAuditMiddleware(RequestDelegate next, ILogger<AdminAuditMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (!context.Request.Path.StartsWithSegments("/api/admin") || context.User.Identity?.IsAuthenticated != true)
            return;

        logger.LogInformation(
            "Admin audit: Method={Method}, Path={Path}, StatusCode={StatusCode}, UserId={UserId}, Roles={Roles}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown",
            string.Join(',', context.User.FindAll(ClaimTypes.Role).Select(x => x.Value)));
    }
}
