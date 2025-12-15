namespace Artix.API.Infra.Identity.Services.SessionService;

using Artix.API.Core.Contract.Primitives.Infra.Identity;
using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Sql.Data.DbContexts;

public sealed class UserSessionService : IUserSessionService
{
    private readonly ArtixCommandDbContext _dbContext;

    public UserSessionService(ArtixCommandDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RecordLoginAsync(
        long userId,
        string jwtId,
        string refreshTokenHash,
        string ipAddress,
        string userAgent,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default)
    {
        var session = UserSession.Create(
            userId: userId,
            jwtId: jwtId,
            refreshTokenHash: refreshTokenHash,
            ipAddress: ipAddress,
            userAgent: userAgent,
            lifetime: lifetime
        );

        _dbContext.UserSessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeByJwtIdAsync(
        string jwtId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.UserSessions
            .FirstOrDefaultAsync(s => s.JwtId == jwtId && s.IsActive, cancellationToken);

        if (session == null)
            return;

        session.Revoke();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.UserSessions
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == refreshTokenHash && s.IsActive, cancellationToken);

        if (session == null)
            return;

        session.Revoke();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllExceptCurrentAsync(
        long userId,
        string currentJwtId,
        CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.UserSessions
            .Where(s => s.UserId == userId && s.JwtId != currentJwtId && s.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        if (sessions.Any())
            await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        if (sessions.Any())
            await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
