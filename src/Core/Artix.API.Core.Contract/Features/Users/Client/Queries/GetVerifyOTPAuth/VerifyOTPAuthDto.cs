namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;

// TODO: add user roles(user subscription)
public sealed record VerifyOTPAuthDto(
    bool IsNewUser,
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
