namespace Artix.API.Infra.Identity.Interfaces.TokenService;

using Core.Contract.Features.Tokens;

public interface ITokenService
{
    Task<JwtTokenResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
