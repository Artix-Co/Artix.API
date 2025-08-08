namespace Artix.API.Infra.Redis.Services;

using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Core.Contract.Features.Caches;

public sealed class RedisCacheService<T> : ICacheService<T> where T : RecentBaseEntity
{
    private readonly IConnectionMultiplexer _redis;
    private readonly string _keyPrefix; // e.g., "recent:museums:user" or "recent:objects:user"
    private readonly int _maxItems;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<RedisCacheService<T>> _logger;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        string keyPrefix,
        ILogger<RedisCacheService<T>> logger,
        int maxItems = 10)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _keyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _maxItems = maxItems;

        _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    public async Task AddToRecentAsync(string userId, T item)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(item);

        var stopwatch = Stopwatch.StartNew();
        var db = _redis.GetDatabase();
        var key = $"{_keyPrefix}:{userId}";
        var serializedItem = JsonSerializer.Serialize(item, _jsonOptions);

        try
        {
            _logger.LogDebug(
                "Attempting to add item with Id {ItemId} for user {UserId} with key {Key}: {SerializedItem}",
                item.Id, userId, key, serializedItem);

            // Check for duplicate by scanning the list
            var items = await db.ListRangeAsync(key, 0, _maxItems - 1);
            bool isDuplicate = false;

            foreach (var existingItem in items)
            {
                if (!existingItem.IsNullOrEmpty)
                {
                    try
                    {
                        var existing = JsonSerializer.Deserialize<T>(existingItem, _jsonOptions);
                        if (existing != null && existing.Id == item.Id)
                        {
                            isDuplicate = true;
                            _logger.LogDebug(
                                "Duplicate item found with Id {ItemId} for user {UserId}. Removing existing item.",
                                item.Id, userId);
                            await db.ListRemoveAsync(key, existingItem, 1); // Remove the duplicate
                            break;
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex,
                            "Failed to deserialize existing item for duplicate check for user {UserId}: {Item}",
                            userId, existingItem);
                    }
                }
            }

            // Add the new item to the front of the list
            await db.ListLeftPushAsync(key, serializedItem);
            await db.ListTrimAsync(key, 0, _maxItems - 1);

            _logger.LogInformation(
                "Successfully added item with Id {ItemId} for user {UserId} with key {Key}. Duplicate: {IsDuplicate}. Time taken: {ElapsedMs}ms",
                item.Id, userId, key, isDuplicate, stopwatch.ElapsedMilliseconds);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while adding item to recent list for user {UserId}.", userId);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    public async Task<List<T>> GetRecentAsync(string userId, int maxItems)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userId);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var db = _redis.GetDatabase();
            var key = $"{_keyPrefix}:{userId}";
            _logger.LogDebug("Fetching recent items for user {UserId} with key {Key}, maxItems: {MaxItems}",
                userId, key, maxItems);

            var items = await db.ListRangeAsync(key, 0, maxItems - 1);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Raw Redis data for key {Key}: {RawData}", key,
                    JsonSerializer.Serialize(items, _jsonOptions));
            }

            var result = new List<T>(items.Length);
            int invalidItems = 0;

            foreach (var item in items)
            {
                if (!item.IsNullOrEmpty)
                {
                    try
                    {
                        var deserialized = JsonSerializer.Deserialize<T>(item, _jsonOptions);
                        if (deserialized != null)
                        {
                            result.Add(deserialized);
                        }
                        else
                        {
                            invalidItems++;
                            _logger.LogWarning("Deserialized item is null for user {UserId}, item: {Item}", userId,
                                item);
                        }
                    }
                    catch (JsonException ex)
                    {
                        invalidItems++;
                        _logger.LogWarning(ex, "Deserialization error for user {UserId}, item: {Item}", userId, item);
                    }
                }
                else
                {
                    invalidItems++;
                    _logger.LogWarning("Empty or null item found in Redis list for user {UserId}, key: {Key}", userId,
                        key);
                }
            }

            _logger.LogInformation(
                "Retrieved {ItemCount} recent items for user {UserId} with key {Key}. Invalid items: {InvalidItems}. Time taken: {ElapsedMs}ms",
                result.Count, userId, key, invalidItems, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while retrieving recent items for user {UserId}.", userId);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    public async Task ClearRecentAsync(string userId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userId);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var db = _redis.GetDatabase();
            var key = $"{_keyPrefix}:{userId}";
            _logger.LogDebug("Clearing recent items for user {UserId} with key {Key}", userId, key);

            await db.KeyDeleteAsync(key);

            _logger.LogInformation(
                "Successfully cleared recent items for user {UserId} with key {Key}. Time taken: {ElapsedMs}ms",
                userId, key, stopwatch.ElapsedMilliseconds);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while clearing recent items for user {UserId}.", userId);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
