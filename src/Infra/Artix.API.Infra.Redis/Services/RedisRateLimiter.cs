namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Configs.Redis;
using Interfaces;
using Microsoft.Extensions.Options;

public sealed class RedisRateLimiter : IRequestRatePolicy
{
    private readonly IRedisConnectionFactory _factory;
    private readonly RateLimitOptions _options;

    public RedisRateLimiter(IRedisConnectionFactory factory, IOptions<RedisOptions> redisOptions)
    {
        _factory = factory;
        _options = redisOptions.Value.RateLimit;
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

        return count <= _options.Limit;
    }
}
