namespace Artix.API.Core.Contract.Features.Users.Queries.GetAccessToken;

public sealed class AccessTokenDto
{
    public string AccessToken { get; init; }
    public DateTime AccessTokenExpiresAt { get; init; }
}
