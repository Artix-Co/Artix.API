namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IRequestRatePolicy
{
    // متدهای اصلی
    Task<bool> IsAllowedAsync(string key, CancellationToken ct = default);
    Task<bool> IsAllowedAsync(string key, int windowSeconds, int limit, CancellationToken ct = default);
    
    // متدهای جدید برای سرویس‌های امنیتی
    Task RecordAttemptAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default);
    Task<int> GetAttemptCountAsync(string key, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(string key, CancellationToken ct = default);
    Task<int> GetRecentCountInLastMinuteAsync(string key, CancellationToken ct = default);
    Task ResetAsync(string key, CancellationToken ct = default);
    
    // متدهای String operations
    Task<string?> GetStringAsync(string key, CancellationToken ct = default);
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task<bool> KeyExistsAsync(string key, CancellationToken ct = default);
    
    // متدهای Set operations
    Task AddToSetAsync(string key, string value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task<HashSet<string>> GetSetMembersAsync(string key, CancellationToken ct = default);
    
    // متدهای List operations
    Task AddToListAsync(string key, string value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task<List<string>> GetListRangeAsync(string key, long start, long stop, CancellationToken ct = default);
    Task TrimListAsync(string key, long keep, CancellationToken ct = default);
}
