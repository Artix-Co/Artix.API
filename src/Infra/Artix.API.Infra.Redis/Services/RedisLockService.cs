namespace Artix.API.Infra.Redis.Services;

using Interfaces;
using StackExchange.Redis;

public sealed class RedisLockService : IDistributedLockService
{
    private readonly IRedisConnectionFactory _factory;
    public RedisLockService(IRedisConnectionFactory factory)
    {
        _factory = factory;
    }
    public async Task<ILockScope?> TryAcquireAsync(string resource, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var db = _factory.Connection.GetDatabase();
        var token = Guid.NewGuid().ToString("N");
        var acquired = await db.LockTakeAsync(resource, token, ttl).WaitAsync(cancellationToken);
        if (!acquired) return null;
        return new RedisLockScope(db, resource, token);
    }
    private sealed class RedisLockScope : ILockScope
    {
        private readonly IDatabase _db;
        private readonly string _resource;
        private readonly string _token;
        public bool Acquired { get; }
        public RedisLockScope(IDatabase db, string resource, string token)
        {
            _db = db;
            _resource = resource;
            _token = token;
            Acquired = true;
        }
        public async ValueTask DisposeAsync()
        {
            await _db.LockReleaseAsync(_resource, _token);
        }
    }
}
