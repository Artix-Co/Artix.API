namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

public class RedisRateLimiter : IRequestRatePolicy
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisRateLimiter> _logger;
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(15);
    
    public RedisRateLimiter(
        IRedisConnectionFactory factory,
        ILogger<RedisRateLimiter> logger)
    {
        _redis = factory.Connection;
        _logger = logger;
    }
    
    // ==========================================
    // متدهای اصلی Rate Limiting
    // ==========================================
    
    public async Task<bool> IsAllowedAsync(string key, CancellationToken ct = default)
    {
        // متد پیش‌فرض با تنظیمات 60 ثانیه و 10 تلاش
        return await IsAllowedAsync(key, 60, 10, ct);
    }
    
    public async Task<bool> IsAllowedAsync(string key, int windowSeconds, int limit, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var windowStart = now - (now % windowSeconds);
            var redisKey = $"rate_limit:{key}:{windowStart}";
            
            // استفاده از Lua script برای atomic operation
            var script = @"
                local current = redis.call('INCR', KEYS[1])
                if current == 1 then
                    redis.call('EXPIRE', KEYS[1], ARGV[1])
                end
                return current
            ";
            
            var count = (long)await db.ScriptEvaluateAsync(
                script,
                new RedisKey[] { redisKey },
                new RedisValue[] { windowSeconds });
            
            var allowed = count <= limit;
            
            if (!allowed)
            {
                _logger.LogDebug(
                    "Rate limit exceeded: Key={Key}, Count={Count}/{Limit}, Window={Window}s",
                    key, count, limit, windowSeconds);
            }
            
            return allowed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rate limit check failed for key: {Key}", key);
            return true; // Fail open
        }
    }
    
    // ==========================================
    // متدهای Attempt Tracking (برای Brute Force)
    // ==========================================
    
    public async Task RecordAttemptAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"attempts:{key}";
            
            // استفاده از Sorted Set برای ذخیره با timestamp
            var score = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await db.SortedSetAddAsync(redisKey, value, score);
            await db.KeyExpireAsync(redisKey, expiry);
            
            _logger.LogDebug("Recorded attempt for key: {Key}, Value: {Value}", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record attempt for key: {Key}", key);
        }
    }
    
    public async Task<int> GetAttemptCountAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"attempts:{key}";
            
            // شمارش تمام تلاش‌ها در محدوده زمانی
            var minScore = DateTimeOffset.UtcNow.AddMinutes(-15).ToUnixTimeSeconds();
            var count = await db.SortedSetLengthAsync(redisKey, minScore, double.PositiveInfinity);
            
            return (int)count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get attempt count for key: {Key}", key);
            return 0;
        }
    }
    
    public async Task<int> GetTotalCountAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"attempts:{key}";
            
            var totalCount = await db.SortedSetLengthAsync(redisKey);
            return (int)totalCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get total count for key: {Key}", key);
            return 0;
        }
    }
    
    public async Task<int> GetRecentCountInLastMinuteAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"attempts:{key}";
            
            var oneMinuteAgo = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds();
            var count = await db.SortedSetLengthAsync(redisKey, oneMinuteAgo, double.PositiveInfinity);
            
            return (int)count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recent count for key: {Key}", key);
            return 0;
        }
    }
    
    public async Task ResetAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"attempts:{key}";
            await db.KeyDeleteAsync(redisKey);
            
            _logger.LogDebug("Reset attempts for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset attempts for key: {Key}", key);
        }
    }
    
    // ==========================================
    // متدهای String Operations
    // ==========================================
    
    public async Task<string?> GetStringAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"string:{key}";
            var value = await db.StringGetAsync(redisKey);
            
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get string for key: {Key}", key);
            return null;
        }
    }
    
    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"string:{key}";
            var expiryTime = expiry ?? _defaultExpiry;
            
            await db.StringSetAsync(redisKey, value, expiryTime);
            
            _logger.LogDebug("Set string for key: {Key}, Expiry: {Expiry}", key, expiryTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set string for key: {Key}", key);
        }
    }
    
    public async Task<bool> KeyExistsAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"string:{key}";
            return await db.KeyExistsAsync(redisKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check key existence for: {Key}", key);
            return false;
        }
    }
    
    // ==========================================
    // متدهای Set Operations (برای Device Management)
    // ==========================================
    
    public async Task AddToSetAsync(string key, string value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"set:{key}";
            
            await db.SetAddAsync(redisKey, value);
            
            if (expiry.HasValue)
            {
                await db.KeyExpireAsync(redisKey, expiry.Value);
            }
            
            _logger.LogDebug("Added to set: Key={Key}, Value={Value}", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add to set for key: {Key}", key);
        }
    }
    
    public async Task<HashSet<string>> GetSetMembersAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"set:{key}";
            var members = await db.SetMembersAsync(redisKey);
            
            return members.Select(m => m.ToString()).ToHashSet();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get set members for key: {Key}", key);
            return new HashSet<string>();
        }
    }
    
    // ==========================================
    // متدهای List Operations (برای تاریخچه)
    // ==========================================
    
    public async Task AddToListAsync(string key, string value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"list:{key}";
            
            // اضافه کردن به انتهای لیست
            await db.ListRightPushAsync(redisKey, value);
            
            // محدود کردن طول لیست به 1000 آیتم
            await db.ListTrimAsync(redisKey, -1000, -1);
            
            if (expiry.HasValue)
            {
                await db.KeyExpireAsync(redisKey, expiry.Value);
            }
            
            _logger.LogDebug("Added to list: Key={Key}, Value={Value}", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add to list for key: {Key}", key);
        }
    }
    
    public async Task<List<string>> GetListRangeAsync(string key, long start, long stop, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"list:{key}";
            var items = await db.ListRangeAsync(redisKey, start, stop);
            
            return items.Select(i => i.ToString()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get list range for key: {Key}", key);
            return new List<string>();
        }
    }
    
    public async Task TrimListAsync(string key, long keep, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"list:{key}";
            
            // نگهداری فقط 'keep' آیتم آخر
            await db.ListTrimAsync(redisKey, -keep, -1);
            
            _logger.LogDebug("Trimmed list for key: {Key}, Keep: {Keep}", key, keep);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trim list for key: {Key}", key);
        }
    }
    
    // ==========================================
    // متدهای کمکی عمومی
    // ==========================================
    
    public async Task<long> IncrementAsync(string key, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"counter:{key}";
            
            var newValue = await db.StringIncrementAsync(redisKey);
            
            if (expiry.HasValue && newValue == 1)
            {
                await db.KeyExpireAsync(redisKey, expiry.Value);
            }
            
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to increment counter for key: {Key}", key);
            return 0;
        }
    }
    
    public async Task<long> GetCounterAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKey = $"counter:{key}";
            var value = await db.StringGetAsync(redisKey);
            
            return value.HasValue ? (long)value : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get counter for key: {Key}", key);
            return 0;
        }
    }
    
    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            
            // حذف از همه namespaceها
            await db.KeyDeleteAsync($"rate_limit:{key}");
            await db.KeyDeleteAsync($"attempts:{key}");
            await db.KeyDeleteAsync($"string:{key}");
            await db.KeyDeleteAsync($"set:{key}");
            await db.KeyDeleteAsync($"list:{key}");
            await db.KeyDeleteAsync($"counter:{key}");
            
            _logger.LogDebug("Deleted all keys for: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete keys for: {Key}", key);
        }
    }
    
    // ==========================================
    // متد Bulk Operations (برای عملکرد بهتر)
    // ==========================================
    
    public async Task<Dictionary<string, string>> GetMultipleStringsAsync(string[] keys, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKeys = keys.Select(k => (RedisKey)$"string:{k}").ToArray();
            var values = await db.StringGetAsync(redisKeys);
            
            var result = new Dictionary<string, string>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (values[i].HasValue)
                {
                    result[keys[i]] = values[i].ToString();
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get multiple strings");
            return new Dictionary<string, string>();
        }
    }
    
    public async Task SetMultipleStringsAsync(Dictionary<string, string> keyValues, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var entries = keyValues.Select(kv => new KeyValuePair<RedisKey, RedisValue>(
                $"string:{kv.Key}", kv.Value)).ToArray();
            
            await db.StringSetAsync(entries);
            
            if (expiry.HasValue)
            {
                foreach (var kv in keyValues)
                {
                    await db.KeyExpireAsync($"string:{kv.Key}", expiry.Value);
                }
            }
            
            _logger.LogDebug("Set multiple strings, Count: {Count}", keyValues.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set multiple strings");
        }
    }
    
    // ==========================================
    // متد Pipeline Operations (برای性能 بهتر)
    // ==========================================
    
    public async Task<List<bool>> CheckMultipleRateLimitsAsync(
        Dictionary<string, (int windowSeconds, int limit)> limits,
        CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var batch = db.CreateBatch();
            var tasks = new List<Task<RedisResult>>();
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            foreach (var kv in limits)
            {
                var windowStart = now - (now % kv.Value.windowSeconds);
                var redisKey = $"rate_limit:{kv.Key}:{windowStart}";
                
                var task = batch.ScriptEvaluateAsync(
                    "local current = redis.call('INCR', KEYS[1]); if current == 1 then redis.call('EXPIRE', KEYS[1], ARGV[1]) end; return current",
                    new RedisKey[] { redisKey },
                    new RedisValue[] { kv.Value.windowSeconds });
                
                tasks.Add(task);
            }
            
            batch.Execute();
            var results = await Task.WhenAll(tasks);
            
            var allowedResults = new List<bool>();
            var limitList = limits.Values.ToList();
            
            for (int i = 0; i < results.Length; i++)
            {
                var count = (long)results[i];
                allowedResults.Add(count <= limitList[i].limit);
            }
            
            return allowedResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check multiple rate limits");
            return limits.Keys.Select(_ => true).ToList(); // Fail open
        }
    }
}
