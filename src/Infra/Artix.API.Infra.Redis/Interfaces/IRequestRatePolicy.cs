namespace Artix.API.Infra.Redis.Interfaces;

public interface IRequestRatePolicy
{
    Task<bool> IsAllowedAsync(string key, CancellationToken ct = default);
}
