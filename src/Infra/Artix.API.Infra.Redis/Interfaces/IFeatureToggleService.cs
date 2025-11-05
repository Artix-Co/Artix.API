namespace Artix.API.Infra.Redis.Interfaces;

public interface IFeatureToggleService
{
    Task<IDictionary<string,string>> GetAllFlagsAsync(CancellationToken ct = default);
    Task<string?> GetFlagAsync(string key, CancellationToken ct = default);
}
