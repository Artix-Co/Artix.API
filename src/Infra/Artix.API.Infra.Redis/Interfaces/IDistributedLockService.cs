namespace Artix.API.Infra.Redis.Interfaces;

public interface ILockScope : IAsyncDisposable
{
    bool Acquired { get; }
}

public interface IDistributedLockService
{
    Task<ILockScope?> TryAcquireAsync(string resource, TimeSpan ttl, CancellationToken cancellationToken = default);
}
