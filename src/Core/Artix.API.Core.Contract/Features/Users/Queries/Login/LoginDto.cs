namespace Artix.API.Core.Contract.Features.Users.Queries.Login;

public sealed record LoginDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string? Username,
    string? DisplayName,
    List<string> Roles);

