namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IEventDeduplicationStore
{
    Task<bool> TryMarkProcessedAsync(string id, int ttlSeconds, CancellationToken ct = default);
}
