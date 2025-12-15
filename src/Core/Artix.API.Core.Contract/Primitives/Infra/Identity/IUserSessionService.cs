namespace Artix.API.Core.Contract.Primitives.Infra.Identity;


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
}
