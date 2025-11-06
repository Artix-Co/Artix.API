namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Configs.Redis;
using Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class RedisRateLimiter : IRequestRatePolicy
{
    private readonly IRedisConnectionFactory _factory;
    private readonly RateLimitOptions _options;
    private readonly ILogger<RedisRateLimiter> _logger;

    public RedisRateLimiter(
        IRedisConnectionFactory factory,
        IOptions<RedisOptions> redisOptions,
        ILogger<RedisRateLimiter> logger)
    {
        _factory = factory;
        _options = redisOptions.Value.RateLimit;
        _logger = logger;
    }

    public async Task<bool> IsAllowedAsync(string key, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var window = TimeSpan.FromSeconds(_options.WindowSeconds);
        var currentWindow = DateTimeOffset.UtcNow.Ticks / window.Ticks;
        var redisKey = $"rate:{key}:{currentWindow}";

        var count = await db.StringIncrementAsync(redisKey);
        if (count == 1)
            await db.KeyExpireAsync(redisKey, window);

        var allowed = count <= _options.Limit;
        _logger.LogInformation(
            "Rate limit check {Key} Window={Window}s Count={Count} Allowed={Allowed}",
            key,
            _options.WindowSeconds,
            count,
            allowed);

        return allowed;
    }
}
