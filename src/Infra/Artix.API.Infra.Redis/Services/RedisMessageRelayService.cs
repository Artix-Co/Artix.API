namespace Artix.API.Infra.Redis.Services;

using Interfaces;
using StackExchange.Redis;

public sealed class RedisMessageRelayService : IMessageRelayService
{
    private readonly IRedisConnectionFactory _factory;
    public RedisMessageRelayService(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task<string> AppendStreamAsync(string stream, string payload, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var id = await db.StreamAddAsync(stream, new NameValueEntry[] { new NameValueEntry("payload", payload) });
        return id;
    }
    public async IAsyncEnumerable<(string Id, string Payload)> ReadStreamAsync(string stream, string fromId, int count, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var entries = await db.StreamReadAsync(stream, fromId, count);
        foreach (var e in entries)
        {
            if (ct.IsCancellationRequested) yield break;
            yield return (e.Id, e.Values.Length > 0 ? e.Values[0].Value.ToString()! : string.Empty);
        }
    }
}
