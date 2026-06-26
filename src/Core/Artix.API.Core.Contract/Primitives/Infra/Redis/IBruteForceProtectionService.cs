namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IBruteForceProtectionService
{
    Task<BruteForceCheckResult> CheckAsync(string identifier, string ipAddress, CancellationToken ct = default);
    Task RecordFailedAttemptAsync(string identifier, string ipAddress, CancellationToken ct = default);
    Task RecordSuccessAsync(string identifier, string ipAddress, CancellationToken ct = default);
    Task ResetAsync(string identifier, string ipAddress, CancellationToken ct = default);
    Task<Dictionary<string, int>> GetStatsAsync(string identifier, CancellationToken ct = default);
}
