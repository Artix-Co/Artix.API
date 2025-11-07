namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Primitives.Infra.Redis;
using StackExchange.Redis;

public sealed class RedisEventDeduplicationStore : IEventDeduplicationStore
{
    private readonly IRedisConnectionFactory _factory;
    public RedisEventDeduplicationStore(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task<bool> TryMarkProcessedAsync(string id, int ttlSeconds, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var added = await db.StringSetAsync($"dedup:{id}", "1", System.TimeSpan.FromSeconds(ttlSeconds), When.NotExists);
        return added;
    }
}
