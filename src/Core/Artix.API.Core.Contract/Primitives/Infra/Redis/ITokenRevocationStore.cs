namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface ITokenRevocationStore
{
    Task RevokeAsync(string jti, DateTimeOffset expiry);
    Task<bool> IsRevokedAsync(string jti);
}
