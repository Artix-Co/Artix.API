namespace Artix.API.Infra.Redis.Interfaces;

public interface ICacheRepository<T>
{
    Task SetAsync(string key, T value, int ttlSeconds);
    Task<T?> GetAsync(string key);
    Task RemoveAsync(string key);
}
