namespace Artix.API.Core.Contract.Features.Users.Queries.GetReNewAccessToken;

public sealed class ReNewAccessTokenDto
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
}
