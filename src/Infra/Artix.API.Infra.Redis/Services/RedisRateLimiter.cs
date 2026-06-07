namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Logging;

public sealed class RedisRateLimiter : IRequestRatePolicy
{
    private readonly IRedisConnectionFactory _factory;

    private readonly ILogger<RedisRateLimiter> _logger;

    public RedisRateLimiter(
        IRedisConnectionFactory factory,
        ILogger<RedisRateLimiter> logger)
    {
        _factory = factory;
        _logger = logger;
    }


    public async Task<bool> IsAllowedAsync(string key, int windowSeconds, int limit, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var window = TimeSpan.FromSeconds(windowSeconds);
        var currentWindow = DateTimeOffset.UtcNow.Ticks / window.Ticks;
        var redisKey = $"rate:{key}:{currentWindow}";

        var count = await db.StringIncrementAsync(redisKey);
        if (count == 1)
            await db.KeyExpireAsync(redisKey, window);

        var allowed = count <= limit;
        _logger.LogInformation(
            "Rate limit check {Key} Window={Window}s Count={Count} Allowed={Allowed}",
            key,
            windowSeconds,
            count,
            allowed);

        return allowed;
    }
}
