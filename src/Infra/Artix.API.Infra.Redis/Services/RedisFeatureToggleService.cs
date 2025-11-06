namespace Artix.API.Infra.Redis.Services;

using Interfaces;

public sealed class RedisFeatureToggleService : IFeatureToggleService
{
    private readonly IRedisConnectionFactory _factory;
    private readonly string _hashKey;
    public RedisFeatureToggleService(IRedisConnectionFactory factory, string hashKey = "feature:flags")
    {
        _factory = factory;
        _hashKey = hashKey;
    }
    public async Task<IDictionary<string,string>> GetAllFlagsAsync(CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var entries = await db.HashGetAllAsync(_hashKey);
        return entries.ToDictionary(e => e.Name.ToString(), e => e.Value.ToString());
    }
    public async Task<string?> GetFlagAsync(string key, CancellationToken ct = default)
    {
        var db = _factory.Connection.GetDatabase();
        var v = await db.HashGetAsync(_hashKey, key);
        return v.IsNullOrEmpty ? null : v.ToString();
    }
}
