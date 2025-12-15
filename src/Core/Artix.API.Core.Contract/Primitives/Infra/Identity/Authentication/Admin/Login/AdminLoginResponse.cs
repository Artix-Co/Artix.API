namespace Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Login;

public record AdminLoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string Username,
    string? DisplayName,
    IReadOnlyList<string> Roles);
