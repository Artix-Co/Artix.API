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
        var redisKey = $"rate:{key}:{_options.WindowSeconds}";
        var count = await db.StringIncrementAsync(redisKey);

        if (count == 1)
            await db.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(_options.WindowSeconds));

        return count <= _options.Limit;
    }
}
