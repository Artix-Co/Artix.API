namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface ILeaderboardService
{
    Task<double> IncrementScoreAsync(string key, string member, double increment, CancellationToken ct = default);
    Task<IList<(string Member, double Score)>> RangeAsync(string key, long start, long stop, CancellationToken ct = default);
}
