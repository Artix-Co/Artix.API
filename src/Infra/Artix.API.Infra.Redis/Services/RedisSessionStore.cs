namespace Artix.API.Infra.Redis.Services;

using Interfaces;
using Microsoft.Extensions.Logging;

public sealed class RedisSessionStore : ISessionStore
{
    private readonly IRedisConnectionFactory _factory;
    private readonly ILogger<RedisSessionStore> _logger;

    public RedisSessionStore(IRedisConnectionFactory factory, ILogger<RedisSessionStore> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task SetSessionAsync(string sessionKey, string json, int ttlSeconds, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var key = $"session:{sessionKey}";
        await db.StringSetAsync(key, json, TimeSpan.FromSeconds(ttlSeconds));
        _logger.LogInformation("Session stored {SessionKey} TTL={TtlSeconds}s", sessionKey, ttlSeconds);
    }

    public async Task<string?> GetSessionAsync(string sessionKey, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var key = $"session:{sessionKey}";
        var value = await db.StringGetAsync(key);
        var result = value.IsNullOrEmpty ? null : value.ToString();
        _logger.LogInformation("Session retrieved {SessionKey} Hit={Hit}", sessionKey, result != null);
        return result;
    }

    public async Task RemoveSessionAsync(string sessionKey, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var key = $"session:{sessionKey}";
        var deleted = await db.KeyDeleteAsync(key);
        _logger.LogInformation("Session removed {SessionKey} Deleted={Deleted}", sessionKey, deleted);
    }
}
