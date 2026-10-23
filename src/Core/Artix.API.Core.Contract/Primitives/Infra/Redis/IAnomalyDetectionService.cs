namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

using Microsoft.AspNetCore.Http;

public interface IAnomalyDetectionService
{
    Task<AnomalyDetectionResult> DetectAsync(
        long userId, 
        string action, 
        HttpContext context,
        CancellationToken ct = default);
    
    Task<AnomalyScore> GetUserTrustScoreAsync(long userId, CancellationToken ct = default);
    Task LogNormalBehaviorAsync(long userId, string action, HttpContext context, CancellationToken ct = default);
}
