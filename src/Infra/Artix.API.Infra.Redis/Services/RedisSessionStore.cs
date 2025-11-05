namespace Artix.API.Infra.Redis.Services;

using Interfaces;

public sealed class RedisSessionStore : ISessionStore
{
    private readonly IRedisConnectionFactory _factory;
    public RedisSessionStore(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task SetSessionAsync(string sessionKey, string json, int ttlSeconds, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        await db.StringSetAsync($"session:{sessionKey}", json, System.TimeSpan.FromSeconds(ttlSeconds));
    }
    public async Task<string?> GetSessionAsync(string sessionKey, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var v = await db.StringGetAsync($"session:{sessionKey}");
        return v.IsNullOrEmpty ? null : v.ToString();
    }
    public async Task RemoveSessionAsync(string sessionKey, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        await db.KeyDeleteAsync($"session:{sessionKey}");
    }
}
