namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth;

using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.DomainServices.OTP;
using Contract.Primitives.DomainServices.OTP.Verify;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetVerifyOTPAuthHandler : QueryHandlerBase<GetVerifyOTPAuthQuery, VerifyOTPAuthDto>
{
    private readonly IOtpService _otpService;

    public GetVerifyOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IOtpService otpService) : base(httpContextAccessor, userManager)
    {
        this._otpService = otpService;
    }

    public override async Task<Result<VerifyOTPAuthDto>> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        var verifyOtpResult = await this._otpService.VerifyAsync(new VerifyOTPRequest(query.PhoneNumber, query.OtpCode),
            cancellationToken);

        var result = new VerifyOTPAuthDto(
            verifyOtpResult.UserId,
            verifyOtpResult.AccessToken,
            verifyOtpResult.RefreshToken,
            verifyOtpResult.AccessTokenExpiration,
            verifyOtpResult.RefreshTokenExpiration
        );

        return Result<VerifyOTPAuthDto>.Success(result);
    }
}
