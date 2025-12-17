namespace Artix.API.Core.Contract.Primitives.DomainServices.OTP.Verify;

public sealed record VerifyOTPRequest(string PhoneNumber, string OtpCode);
