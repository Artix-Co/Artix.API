namespace Artix.API.Infra.Redis.Services;

using Interfaces;

public sealed class RedisJobQueueService : IBackgroundJobScheduler
{
    private readonly IRedisConnectionFactory _factory;
    public RedisJobQueueService(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task EnqueueAsync(string queueName, string payload, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        await db.ListLeftPushAsync($"queue:{queueName}", payload);
    }
    public async Task<string?> DequeueBlockingAsync(string queueName, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var res = await db.ListRightPopAsync($"queue:{queueName}");
        return res.IsNullOrEmpty ? null : res.ToString();
    }
}
