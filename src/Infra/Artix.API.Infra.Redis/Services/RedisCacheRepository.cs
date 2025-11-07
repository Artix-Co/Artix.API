namespace Artix.API.Infra.Redis.Services;

using System.Diagnostics;
using System.Text.Json;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Logging;

public sealed class RedisCacheRepository<T> : ICacheRepository<T>
{
    private readonly IRedisConnectionFactory _factory;
    private readonly ILogger<RedisCacheRepository<T>> _logger;

    public RedisCacheRepository(IRedisConnectionFactory factory, ILogger<RedisCacheRepository<T>> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task SetAsync(string key, T value, int ttlSeconds)
    {
        var db = _factory.Connection.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        var success = await db.StringSetAsync(key, json, TimeSpan.FromSeconds(ttlSeconds));

        _logger.LogInformation(
            "Cache set {CacheType} Key={CacheKey} TTL={TtlSeconds}s Success={Success}",
            typeof(T).Name, key, ttlSeconds, success);
    }

    public async Task<T?> GetAsync(string key)
    {
        var db = _factory.Connection.GetDatabase();
        var start = Stopwatch.GetTimestamp();
        var value = await db.StringGetAsync(key);
        var elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;

        if (value.IsNullOrEmpty)
        {
            _logger.LogInformation(
                "Cache miss {CacheType} Key={CacheKey} LatencyMs={LatencyMs:F2}",
                typeof(T).Name, key, elapsedMs);
            return default;
        }

        var result = JsonSerializer.Deserialize<T>(value!);
        _logger.LogInformation(
            "Cache hit {CacheType} Key={CacheKey} LatencyMs={LatencyMs:F2}",
            typeof(T).Name, key, elapsedMs);
        return result;
    }

    public async Task RemoveAsync(string key)
    {
        var db = _factory.Connection.GetDatabase();
        var deleted = await db.KeyDeleteAsync(key);
        _logger.LogInformation(
            "Cache remove {CacheType} Key={CacheKey} Deleted={Deleted}",
            typeof(T).Name, key, deleted);
    }


    public async Task<T?> GetOrSetAsync(
        string key,
        Func<Task<T>> factory,
        int ttlSeconds,
        CancellationToken ct = default)
    {
        var cached = await GetAsync(key);
        if (cached is not null)
            return cached;

        var value = await factory();

        if (value is not null)
            await SetAsync(key, value, ttlSeconds);

        return value;
    }
}
