namespace Artix.API.Infra.Redis.Interfaces;

public interface ICacheService<T>
{
    Task AddToRecentAsync(string userId, T item);
    Task<List<T>> GetRecentAsync(string userId, int maxItems);
    Task ClearRecentAsync(string userId);
}
