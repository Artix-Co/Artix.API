namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IFeatureToggleService
{
    Task<IDictionary<string,string>> GetAllFlagsAsync(CancellationToken ct = default);
    Task<string?> GetFlagAsync(string key, CancellationToken ct = default);
}
