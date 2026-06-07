namespace Artix.API.Endpoints.Middlewares;

using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(
        RequestDelegate next,
        IServiceProvider serviceProvider,
        ILogger<RateLimitingMiddleware> logger)
    {
        this._next = next;
        this._serviceProvider = serviceProvider;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // لاگ برای دیباگ - حتماً این خط را ببینید
        this._logger.LogInformation($"[RATE-LIMIT] Request to: {context.Request.Path}");

        var endpoint = context.GetEndpoint();
        this._logger.LogInformation($"[RATE-LIMIT] Endpoint: {endpoint?.DisplayName ?? "NULL"}");

        var rateLimitAttribute = endpoint?.Metadata.GetMetadata<RateLimitAttribute>();
        this._logger.LogInformation($"[RATE-LIMIT] Attribute found: {rateLimitAttribute != null}");

        if (rateLimitAttribute != null)
        {
            this._logger.LogInformation(
                $"[RATE-LIMIT] KeyPrefix: {rateLimitAttribute.KeyPrefix}, Limit: {rateLimitAttribute.Limit}, Window: {rateLimitAttribute.WindowSeconds}");


            using var scope = this._serviceProvider.CreateScope();
            var rateLimiter = scope.ServiceProvider.GetRequiredService<IRequestRatePolicy>();

            var key = rateLimitAttribute.GetRateLimitKey(context);
            var isAllowed =
                await rateLimiter.IsAllowedAsync(key, rateLimitAttribute.WindowSeconds, rateLimitAttribute.Limit);

            if (!isAllowed)
            {
                this._logger.LogWarning("Rate limit exceeded for key: {Key}", key);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests. Please try again later.",
                    retryAfter = rateLimitAttribute.WindowSeconds
                });
                return;
            }
        }

        await this._next(context);
    }
}
