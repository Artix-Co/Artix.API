namespace Artix.API.Core.Contract.Features.Users.Queries.GetReNewAccessToken;

public sealed record ReNewAccessTokenDto(string AccessToken, DateTime AccessTokenExpiresAt);
