namespace Artix.API.Infra.Redis.Services;

using Interfaces;
using StackExchange.Redis;

public sealed class RedisLeaderboardService : ILeaderboardService
{
    private readonly IRedisConnectionFactory _factory;
    public RedisLeaderboardService(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task<double> IncrementScoreAsync(string key, string member, double increment, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        return await db.SortedSetIncrementAsync(key, member, increment);
    }
    public async Task<IList<(string Member, double Score)>> RangeAsync(string key, long start, long stop, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var entries = await db.SortedSetRangeByRankWithScoresAsync(key, start, stop, Order.Descending);
        return entries.Select(e => (e.Element.ToString()!, e.Score)).ToList();
    }
}
