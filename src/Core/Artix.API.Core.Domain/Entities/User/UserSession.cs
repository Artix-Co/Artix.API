namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Exceptions;

public class UserSession : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; } = null!;

    // Session
    public Guid SessionId { get; private set; }

    // JWT
    public string JwtId { get; private set; } = null!;

    // Refresh Token
    public Guid RefreshTokenFamilyId { get; private set; }
    public string RefreshTokenHash { get; private set; } = null!;
    public DateTime RefreshTokenExpiresAt { get; private set; }

    // Device
    public string DeviceId { get; private set; } = null!;
    public string Fingerprint { get; private set; } = null!;

    // Client
    public string IpAddress { get; private set; } = null!;
    public string UserAgent { get; private set; } = null!;

    // Audit
    public DateTime LastSeenAt { get; private set; }

    // Revocation
    public DateTime? RevokedAt { get; private set; }
    public string? RevokeReason { get; private set; }
    public Guid? ReplacedBySessionId { get; private set; }

    public DateTime? LastRefreshAt { get; private set; }

    public int RefreshCount { get; private set; }

    public bool IsActive =>
        RevokedAt is null &&
        DateTime.UtcNow < RefreshTokenExpiresAt;

    private UserSession()
    {
    }

    public static UserSession Create(
        long userId,
        Guid sessionId,
        Guid familyId,
        string jwtId,
        string refreshTokenHash,
        DateTime refreshTokenExpiresAt,
        string deviceId,
        string fingerprint,
        string ipAddress,
        string userAgent)
    {
        return new UserSession
        {
            UserId = userId,
            SessionId = sessionId,
            RefreshTokenFamilyId = familyId,
            JwtId = jwtId,
            RefreshTokenHash = refreshTokenHash,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            DeviceId = deviceId,
            Fingerprint = fingerprint,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow
        };
    }

    public void Touch()
    {
        LastSeenAt = DateTime.UtcNow;
    }

    public void Revoke(string reason)
    {
        RevokedAt = DateTime.UtcNow;
        RevokeReason = reason;
    }

    public void Replace(Guid newSessionId)
    {
        ReplacedBySessionId = newSessionId;
    }

    public void RecordRefresh()
    {
        LastRefreshAt = DateTime.UtcNow;
        RefreshCount++;
    }

    public void UpdateLastSeen()
    {
        LastSeenAt = DateTime.UtcNow;
    }



    public void RecordRefresh(DateTime utcNow)
    {
        LastRefreshAt = utcNow;
        RefreshCount++;
    }
    public void Rotate(
        string newJwtId,
        string newRefreshTokenHash,
        DateTime refreshTokenExpiresAt,
        DateTime utcNow)
    {
        JwtId = newJwtId;
        RefreshTokenHash = newRefreshTokenHash;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;

        LastRefreshAt = utcNow;
        LastSeenAt = utcNow;

        RefreshCount++;
    }

    public void UpdateLastSeen(DateTime utcNow)
    {
        LastSeenAt = utcNow;
    }

    public void Revoke(
        string reason,
        DateTime utcNow)
    {
        RevokedAt = utcNow;
        RevokeReason = reason;
    }
}
