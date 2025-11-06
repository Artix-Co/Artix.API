namespace Artix.API.Infra.Redis.Interfaces;

public interface IEventDeduplicationStore
{
    Task<bool> TryMarkProcessedAsync(string id, int ttlSeconds, CancellationToken ct = default);
}
