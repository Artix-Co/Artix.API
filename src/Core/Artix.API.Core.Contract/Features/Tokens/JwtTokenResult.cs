namespace Artix.API.Core.Contract.Features.Tokens;

public sealed class JwtTokenResult
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime AccessTokenExpiresAt { get; init; }
    public DateTime RefreshTokenExpiresAt { get; init; }
}
