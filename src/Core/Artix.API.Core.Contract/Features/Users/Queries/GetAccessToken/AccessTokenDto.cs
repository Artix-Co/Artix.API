namespace Artix.API.Core.Contract.Features.Users.Queries.GetAccessToken;

public sealed class AccessTokenDto
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
}
