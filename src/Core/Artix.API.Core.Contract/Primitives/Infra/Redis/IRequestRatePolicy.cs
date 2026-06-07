namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IRequestRatePolicy
{
    Task<bool> IsAllowedAsync(string key, int windowSeconds, int limit, CancellationToken ct = default);
}
