namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin;

public sealed record AdminLoginDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string? Username,
    string? DisplayName,
    IReadOnlyList<string> Roles);

