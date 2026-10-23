namespace Artix.API.Infra.Identity.Services.SessionService;

using System.Security;
using Core.Contract.Primitives.Infra.Identity;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Sql.Data.DbContexts;
using Microsoft.Extensions.Logging;

internal sealed class UserSessionService : IUserSessionService
{
    private readonly ArtixCommandDbContext _dbContext;
    private readonly ITokenRevocationStore _revocationStore;
    private readonly ILogger<UserSessionService> _logger;
    private readonly TimeProvider _timeProvider;

    public UserSessionService(
        ArtixCommandDbContext dbContext,
        ITokenRevocationStore revocationStore,
        ILogger<UserSessionService> logger,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _revocationStore = revocationStore;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task<UserSession> CreateAsync(
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
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        _logger.LogInformation(
            "Creating session. UserId={UserId}, SessionId={SessionId}",
            userId,
            sessionId);

        var exists = await _dbContext.UserSessions
            .AsNoTracking()
            .AnyAsync(x =>
                    x.SessionId == sessionId ||
                    x.JwtId == jwtId,
                cancellationToken);

        if (exists)
        {
            _logger.LogError(
                "Duplicate session detected. UserId={UserId}, SessionId={SessionId}",
                userId,
                sessionId);

            throw new SecurityException(
                "Duplicate session detected.");
        }

        var session = UserSession.Create(
            userId: userId,
            sessionId: sessionId,
            familyId: familyId,
            jwtId: jwtId,
            refreshTokenHash: refreshTokenHash,
            refreshTokenExpiresAt: refreshTokenExpiresAt,
            deviceId: deviceId,
            fingerprint: fingerprint,
            ipAddress: ipAddress,
            userAgent: userAgent);

        _dbContext.UserSessions.Add(session);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Session created successfully. UserId={UserId}, SessionId={SessionId}",
            userId,
            sessionId);

        return session;
    }

    public async Task<UserSession?> FindByJwtIdAsync(
        string jwtId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _dbContext.UserSessions
            .FirstOrDefaultAsync(
                x => x.JwtId == jwtId,
                cancellationToken);
    }

    public async Task<UserSession?> FindByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _dbContext.UserSessions
            .FirstOrDefaultAsync(
                x => x.RefreshTokenHash == refreshTokenHash,
                cancellationToken);
    }

    public async Task<UserSession?> GetCurrentAsync(
        long userId,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        return await _dbContext.UserSessions
            .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SessionId == sessionId &&
                    x.RevokedAt == null &&
                    x.RefreshTokenExpiresAt > now,
                cancellationToken);
    }


    public async Task RotateRefreshTokenAsync(
        Guid sessionId,
        string newJwtId,
        string newRefreshTokenHash,
        DateTime refreshExpiresAt,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var session = await _dbContext.UserSessions
            .FirstOrDefaultAsync(
                x => x.SessionId == sessionId,
                cancellationToken);

        if (session is null)
            throw new SecurityException("Session not found.");

        if (!session.IsActive)
            throw new SecurityException("Session is no longer active.");

        // جلوگیری از ثبت JTI تکراری
        var duplicateJwt = await _dbContext.UserSessions
            .AsNoTracking()
            .AnyAsync(
                x => x.JwtId == newJwtId &&
                     x.SessionId != sessionId,
                cancellationToken);

        if (duplicateJwt)
            throw new SecurityException("Duplicate JWT id.");

        // AccessToken قبلی دیگر معتبر نیست
        await _revocationStore.RevokeAsync(
            session.JwtId,
            session.RefreshTokenExpiresAt,
            cancellationToken);

        session.Rotate(
            newJwtId,
            newRefreshTokenHash,
            refreshExpiresAt,
            now);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Session rotated. UserId={UserId}, SessionId={SessionId}",
            session.UserId,
            session.SessionId);
    }

    public async Task TouchAsync(
        string jwtId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var session = await _dbContext.UserSessions
            .FirstOrDefaultAsync(
                x => x.JwtId == jwtId,
                cancellationToken);

        if (session is null)
            return;

        if (!session.IsActive)
            return;

        session.UpdateLastSeen(
            _timeProvider.GetUtcNow().UtcDateTime);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAsync(
        Guid sessionId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var session = await _dbContext.UserSessions
            .FirstOrDefaultAsync(
                x => x.SessionId == sessionId,
                cancellationToken);

        if (session is null)
            return;

        if (session.RevokedAt != null)
            return;

        session.Revoke(reason, now);

        await _revocationStore.RevokeAsync(
            session.JwtId,
            session.RefreshTokenExpiresAt,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Session revoked. UserId={UserId}, SessionId={SessionId}",
            session.UserId,
            session.SessionId);
    }

    public async Task RevokeByJwtIdAsync(
        string jwtId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var session = await _dbContext.UserSessions
            .FirstOrDefaultAsync(
                x => x.JwtId == jwtId,
                cancellationToken);

        if (session is null)
            return;

        if (session.RevokedAt != null)
            return;

        session.Revoke(reason, now);

        await _revocationStore.RevokeAsync(
            session.JwtId,
            session.RefreshTokenExpiresAt,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "JWT revoked. Jti={JwtId}", jwtId);
    }

    public async Task RevokeFamilyAsync(
        Guid familyId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var sessions = await _dbContext.UserSessions
            .Where(x =>
                x.RefreshTokenFamilyId == familyId &&
                x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        if (sessions.Count == 0)
            return;

        foreach (var session in sessions)
        {
            session.Revoke(reason, now);

            await _revocationStore.RevokeAsync(
                session.JwtId,
                session.RefreshTokenExpiresAt,
                cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "Refresh token family revoked. FamilyId={FamilyId}, Count={Count}",
            familyId,
            sessions.Count);
    }

    public async Task RevokeAllAsync(
        long userId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var sessions = await _dbContext.UserSessions
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        if (sessions.Count == 0)
            return;

        foreach (var session in sessions)
        {
            session.Revoke(reason, now);

            await _revocationStore.RevokeAsync(
                session.JwtId,
                session.RefreshTokenExpiresAt,
                cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "All sessions revoked. UserId={UserId}, Count={Count}",
            userId,
            sessions.Count);
    }

    public async Task<UserSession?> GetLastSessionByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _dbContext.UserSessions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<bool> IsValidSessionAsync(
        long userId,
        string jwtId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // ابتدا Redis را چک کن
        if (await _revocationStore.IsRevokedAsync(jwtId, cancellationToken))
            return false;

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        return await _dbContext.UserSessions
            .AsNoTracking()
            .AnyAsync(x =>
                    x.UserId == userId &&
                    x.JwtId == jwtId &&
                    x.RevokedAt == null &&
                    x.RefreshTokenExpiresAt > now,
                cancellationToken);
    }
}
