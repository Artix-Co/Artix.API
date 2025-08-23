namespace Artix.API.Core.Contract.Features.Users.Queries.VerifyOTPAuth;

// TODO: add user roles(user subscription)
public sealed record VerifyOTPAuthDto(
    bool IsNewUser,
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
