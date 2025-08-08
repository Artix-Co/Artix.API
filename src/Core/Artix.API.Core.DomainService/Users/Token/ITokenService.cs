namespace Artix.API.Core.DomainService.Users.Token;

using Contract.Features.Tokens;

public interface ITokenService
{
    Task<JwtTokenResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
