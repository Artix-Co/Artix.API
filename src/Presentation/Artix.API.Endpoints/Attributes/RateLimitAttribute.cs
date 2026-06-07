namespace Artix.API.Endpoints.Attributes;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : Attribute
{
    public string KeyPrefix { get; set; }
    public int WindowSeconds { get; set; }
    public int Limit { get; set; }
    public bool UseClientIp { get; set; } = true;
    public bool UseUserIdentifier { get; set; } = false;

    public RateLimitAttribute(string keyPrefix, int windowSeconds, int limit)
    {
        this.KeyPrefix = keyPrefix;
        this.WindowSeconds = windowSeconds;
        this.Limit = limit;
    }

    public string GetRateLimitKey(HttpContext context)
    {
        var key = this.KeyPrefix;

        if (this.UseClientIp)
        {
            var ip = this.GetClientIp(context);
            key = $"{key}:ip:{ip}";
        }

        if (this.UseUserIdentifier && context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            key = $"{key}:user:{userId}";
        }

        return key;
    }

    private string GetClientIp(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.FirstOrDefault()?.Split(',').FirstOrDefault() ?? "unknown";

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
