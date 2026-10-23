namespace Artix.API.Core.Contract.Primitives.Infra.Identity;

using Domain.Entities.User;
public interface IUserSessionService
{
    Task<UserSession> CreateAsync(
        long userId,
        Guid sessionId,
        Guid familyId,
        string jwtId,
        string refreshTokenHash,
        DateTime refreshTokenExpiresAt,
        string deviceId,
        string fingerprint,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken = default);

    Task<UserSession?> FindByJwtIdAsync(
        string jwtId,
        CancellationToken cancellationToken = default);

    Task<UserSession?> FindByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default);

    Task<UserSession?> GetCurrentAsync(
        long userId,
        Guid sessionId,
        CancellationToken cancellationToken = default);

    Task<bool> IsValidSessionAsync(
        long userId,
        string jwtId,
        CancellationToken cancellationToken = default);

    Task TouchAsync(
        string jwtId,
        CancellationToken cancellationToken = default);

    Task RotateRefreshTokenAsync(
        Guid sessionId,
        string newJwtId,
        string newRefreshTokenHash,
        DateTime refreshExpiresAt,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(
        Guid sessionId,
        string reason,
        CancellationToken cancellationToken = default);

    Task RevokeByJwtIdAsync(
        string jwtId,
        string reason,
        CancellationToken cancellationToken = default);

    Task RevokeFamilyAsync(
        Guid familyId,
        string reason,
        CancellationToken cancellationToken = default);

    Task RevokeAllAsync(
        long userId,
        string reason,
        CancellationToken cancellationToken = default);

    Task<UserSession?> GetLastSessionByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
}
