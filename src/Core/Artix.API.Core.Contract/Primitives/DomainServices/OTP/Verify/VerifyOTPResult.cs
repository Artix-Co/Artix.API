namespace Artix.API.Core.Contract.Primitives.DomainServices.OTP.Verify;

public sealed record VerifyOTPResult(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration);
