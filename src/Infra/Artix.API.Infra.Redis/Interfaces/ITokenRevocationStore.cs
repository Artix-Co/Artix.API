namespace Artix.API.Infra.Redis.Interfaces;

public interface ITokenRevocationStore
{
    Task RevokeAsync(string jti, DateTimeOffset expiry);
    Task<bool> IsRevokedAsync(string jti);
}
