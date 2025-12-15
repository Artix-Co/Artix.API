namespace Artix.API.Infra.Identity.Services.SessionService;

using Artix.API.Core.Contract.Primitives.Infra.Identity;
using Core.Domain.Entities.User;
using Sql.Data.DbContexts;

public sealed class UserSessionService : IUserSessionService
{
    private readonly ArtixCommandDbContext _dbContext;

    public UserSessionService(ArtixCommandDbContext dbContext)
    {
        this._dbContext = dbContext;
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

        this._dbContext.UserSessions.Add(session);
        await this._dbContext.SaveChangesAsync(cancellationToken);
    }
}
