namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetReNewAccessToken;

public sealed record ReNewAccessTokenDto(string AccessToken, DateTime AccessTokenExpiresAt);
