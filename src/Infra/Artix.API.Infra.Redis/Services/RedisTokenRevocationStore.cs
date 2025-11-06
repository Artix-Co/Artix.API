namespace Artix.API.Infra.Redis.Services;

using Interfaces;

public sealed class RedisTokenRevocationStore : ITokenRevocationStore
{
    private readonly IRedisConnectionFactory _factory;
    public RedisTokenRevocationStore(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task RevokeAsync(string jti, DateTimeOffset expiry)
    {
        var db = _factory.Connection.GetDatabase();
        var ttl = expiry - DateTimeOffset.UtcNow;
        if (ttl <= TimeSpan.Zero) ttl = TimeSpan.FromSeconds(1);
        await db.StringSetAsync($"revoked:{jti}", "1", ttl);
    }
    public async Task<bool> IsRevokedAsync(string jti)
    {
        var db = _factory.Connection.GetDatabase();
        return await db.KeyExistsAsync($"revoked:{jti}");
    }
}
