namespace Artix.API.Core.Contract.Primitives.Infra.Identity;

public sealed class JwtTokenResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
