namespace Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Client.Login;

public record ClientLoginResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
