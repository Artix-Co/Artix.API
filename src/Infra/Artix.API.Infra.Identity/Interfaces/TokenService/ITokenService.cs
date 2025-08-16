namespace Artix.API.Infra.Identity.Interfaces.TokenService;

using Core.Contract.Features.Tokens;

public interface ITokenService
{
    Task<JwtTokenResult> ReNewAccessTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default);
    
}
