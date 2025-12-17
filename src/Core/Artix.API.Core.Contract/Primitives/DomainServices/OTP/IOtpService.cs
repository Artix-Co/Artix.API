namespace Artix.API.Core.Contract.Primitives.DomainServices.OTP;

using Init;
using Verify;

public interface IOtpService
{
    Task<InitOTPResult> InitAsync(InitOTPRequest request, CancellationToken cancellationToken = default);
    Task<VerifyOTPResult> VerifyAsync(VerifyOTPRequest request, CancellationToken cancellationToken = default);
}
