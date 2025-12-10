namespace Artix.API.Core.Contract.Primitives.Infra.Identity;

public interface ITokenService
{
    Task<JwtTokenResult> ReNewAccessTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default);
    
}
