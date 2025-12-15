namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Exceptions;

public class UserSession : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }

    public string JwtId { get; private set; }
    public string RefreshTokenHash { get; private set; }

    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }


    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsActive =>
        RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    protected UserSession()
    {
    }

    private UserSession(
        long userId,
        string jwtId,
        string refreshTokenHash,
        string ipAddress,
        string userAgent,
        TimeSpan lifetime)
    {
        if (userId <= 0)
            throw DomainException.InvalidValue(nameof(userId));

        if (string.IsNullOrWhiteSpace(jwtId))
            throw DomainException.InvalidValue(nameof(JwtId));

        if (string.IsNullOrWhiteSpace(refreshTokenHash))
            throw DomainException.InvalidValue(nameof(RefreshTokenHash));

        if (string.IsNullOrWhiteSpace(ipAddress))
            throw DomainException.InvalidValue(nameof(IpAddress));

        if (string.IsNullOrWhiteSpace(userAgent))
            throw DomainException.InvalidValue(nameof(UserAgent));

        if (lifetime <= TimeSpan.Zero)
            throw DomainException.InvalidOperation("Session lifetime must be positive");


        UserId = userId;
        JwtId = jwtId;
        RefreshTokenHash = refreshTokenHash;
        IpAddress = ipAddress;
        UserAgent = userAgent;


        ExpiresAt = CreatedAt.Add(lifetime);
    }


    // ----------------------------
    // Factory Method
    // ----------------------------
    public static UserSession Create(
        long userId,
        string jwtId,
        string refreshTokenHash,
        string ipAddress,
        string userAgent,
        TimeSpan lifetime)
    {
        return new UserSession(
            userId,
            jwtId,
            refreshTokenHash,
            ipAddress,
            userAgent,
            lifetime);
    }

    // ----------------------------
    // Domain Behaviors
    // ----------------------------
    public void Revoke()
    {
        if (RevokedAt != null)
            return;

        RevokedAt = DateTime.UtcNow;
    }

    public void Extend(TimeSpan lifetime)
    {
        if (!IsActive)
            throw DomainException.InvalidOperation("Cannot extend inactive session");

        if (lifetime <= TimeSpan.Zero)
            throw DomainException.InvalidOperation("Session lifetime must be positive");

        ExpiresAt = DateTime.UtcNow.Add(lifetime);
    }

    public void RotateRefreshToken(string newHash)
    {
        if (!IsActive)
            throw DomainException.InvalidOperation("Cannot rotate token for inactive session");

        if (string.IsNullOrWhiteSpace(newHash))
            throw DomainException.InvalidValue(nameof(RefreshTokenHash));

        RefreshTokenHash = newHash;
    }
}
