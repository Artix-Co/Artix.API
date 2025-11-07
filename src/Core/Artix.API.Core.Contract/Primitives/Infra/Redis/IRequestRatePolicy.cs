namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IRequestRatePolicy
{
    Task<bool> IsAllowedAsync(string key, CancellationToken ct = default);
}
