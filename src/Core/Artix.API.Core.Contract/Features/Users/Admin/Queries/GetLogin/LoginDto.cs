namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin;

public sealed record LoginDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string? Username,
    string? DisplayName,
    List<string> Roles);

