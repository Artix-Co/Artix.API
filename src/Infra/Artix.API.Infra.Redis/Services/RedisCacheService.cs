namespace Artix.API.Infra.Redis.Services;

using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;
using Interfaces;

public sealed class RedisCacheService<T> : ICacheService<T>
{
    private readonly IConnectionMultiplexer _redis;
    private readonly string _keyPrefix; // e.g., "recent:museums:user" or "recent:objects:user"
    private readonly int _maxItems;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis, string keyPrefix, int maxItems = 10)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _keyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
        _maxItems = maxItems;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task AddToRecentAsync(string userId, T item)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        var db = _redis.GetDatabase();
        var key = $"{_keyPrefix}:{userId}";
        var serializedItem = JsonSerializer.Serialize(item, _jsonOptions);

        await db.ListLeftPushAsync(key, serializedItem);
        await db.ListTrimAsync(key, 0, _maxItems - 1);
    }

    public async Task<List<T>> GetRecentAsync(string userId, int maxItems)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        var db = _redis.GetDatabase();
        var key = $"{_keyPrefix}:{userId}";
        var items = await db.ListRangeAsync(key, 0, maxItems - 1);

        // Log raw Redis data for debugging
        Console.WriteLine($"Raw Redis data for key {key}: {JsonSerializer.Serialize(items, _jsonOptions)}");

        var result = new List<T>();
        foreach (var item in items)
        {
            if (item.HasValue)
            {
                try
                {
                    var deserialized = JsonSerializer.Deserialize<T>(item, _jsonOptions);
                    if (deserialized != null)
                    {
                        result.Add(deserialized);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Deserialization error for item {item}: {ex.Message}");
                }
            }
        }

        return result;
    }

    public async Task ClearRecentAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        var db = _redis.GetDatabase();
        var key = $"{_keyPrefix}:{userId}";
        await db.KeyDeleteAsync(key);
    }
}
