namespace Artix.API.Infra.Redis.Interfaces;

public interface ISessionStore
{
    Task SetSessionAsync(string sessionKey, string json, int ttlSeconds, CancellationToken ct = default);
    Task<string?> GetSessionAsync(string sessionKey, CancellationToken ct = default);
    Task RemoveSessionAsync(string sessionKey, CancellationToken ct = default);
}
