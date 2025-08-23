namespace Artix.API.Infra.Redis.Interfaces;

using Core.Contract.Features.Caches;

public interface ICacheService<T> where T : RecentBaseEntity
{
    Task AddToRecentAsync(string userId, T item);
    Task<List<T>> GetRecentAsync(string userId, int maxItems = 10);
    Task ClearRecentAsync(string userId);
}
