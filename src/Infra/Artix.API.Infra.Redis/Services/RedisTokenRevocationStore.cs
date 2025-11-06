namespace Artix.API.Infra.Redis.Services;

using Interfaces;
using Microsoft.Extensions.Logging;

public sealed class RedisTokenRevocationStore : ITokenRevocationStore
{
    private readonly IRedisConnectionFactory _factory;
    private readonly ILogger<RedisTokenRevocationStore> _logger;

    public RedisTokenRevocationStore(IRedisConnectionFactory factory, ILogger<RedisTokenRevocationStore> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task RevokeAsync(string jti, DateTimeOffset expiry)
    {
        var db = _factory.Connection.GetDatabase();
        var key = $"revoked:{jti}";
        var ttl = expiry - DateTimeOffset.UtcNow;
        if (ttl <= TimeSpan.Zero) ttl = TimeSpan.FromSeconds(1);
        await db.StringSetAsync(key, "1", ttl);
        _logger.LogWarning("Token revoked JTI={Jti} TTL={TtlSeconds}s", jti, ttl.TotalSeconds);
    }

    public async Task<bool> IsRevokedAsync(string jti)
    {
        var db = _factory.Connection.GetDatabase();
        var key = $"revoked:{jti}";
        var exists = await db.KeyExistsAsync(key);
        _logger.LogInformation("Token revocation check JTI={Jti} Revoked={Revoked}", jti, exists);
        return exists;
    }
}
