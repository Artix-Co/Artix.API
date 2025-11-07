namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface ICacheRepository<T>
{
    Task SetAsync(string key, T value, int ttlSeconds);
    Task<T?> GetAsync(string key);
    Task RemoveAsync(string key);
    

    Task<T?> GetOrSetAsync(
        string key,
        Func<Task<T>> factory,
        int ttlSeconds,
        CancellationToken ct = default);
}
