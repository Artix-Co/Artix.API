namespace Artix.API.Core.Contract.Primitives.Infra.Identity;

using Domain.Entities.User;

public interface IUserSessionService
{
    Task RecordLoginAsync(
        long userId,
        string jwtId,
        string refreshTokenHash,
        string ipAddress,
        string userAgent,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default);

    Task RevokeByJwtIdAsync(
        string jwtId,
        CancellationToken cancellationToken = default);

    Task RevokeByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default);

    Task RevokeAllExceptCurrentAsync(
        long userId,
        string currentJwtId,
        CancellationToken cancellationToken = default);

    Task RevokeAllAsync(
        long userId,
        CancellationToken cancellationToken = default);
    
    Task<bool> IsValidSessionAsync(long userId, string jwtId, CancellationToken cancellationToken = default);
    Task<UserSession?> GetActiveSessionByJwtIdAsync(string jwtId, CancellationToken cancellationToken = default);

    Task<UserSession?> GetLastSessionByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
}
