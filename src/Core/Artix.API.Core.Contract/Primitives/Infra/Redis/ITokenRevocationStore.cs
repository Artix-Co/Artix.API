namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface ITokenRevocationStore
{
    Task RevokeAsync(string jti, DateTimeOffset expiry, CancellationToken cancellationToken = default);
    Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default);
}
