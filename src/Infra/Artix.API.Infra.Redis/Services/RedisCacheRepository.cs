namespace Artix.API.Infra.Redis.Services;

using System.Text.Json;
using Interfaces;

public sealed class RedisCacheRepository<T> : ICacheRepository<T>
{
    private readonly IRedisConnectionFactory _factory;
    public RedisCacheRepository(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task SetAsync(string key, T value, int ttlSeconds)
    {
        var db = _factory.Connection.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, TimeSpan.FromSeconds(ttlSeconds));
    }
    public async Task<T?> GetAsync(string key)
    {
        var db = _factory.Connection.GetDatabase();
        var v = await db.StringGetAsync(key);
        if (v.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(v!);
    }
    public async Task RemoveAsync(string key)
    {
        var db = _factory.Connection.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
