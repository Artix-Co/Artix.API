namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth;

using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.DomainServices.OTP;
using Contract.Primitives.DomainServices.OTP.Verify;
using Contract.Primitives.Models;
using Contract.Primitives.Infra.Redis;
using Exceptions;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class GetVerifyOTPAuthHandler : QueryHandlerBase<GetVerifyOTPAuthQuery, VerifyOTPAuthDto>
{
    private readonly IOtpService _otpService;
    private readonly IRequestRatePolicy _ratePolicy;

    public GetVerifyOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IOtpService otpService, IRequestRatePolicy ratePolicy) : base(httpContextAccessor, userManager)
    {
        this._otpService = otpService;
        _ratePolicy = ratePolicy;
    }

    public override async Task<Result<VerifyOTPAuthDto>> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        if (!await _ratePolicy.IsAllowedAsync($"otp:verify:phone:{query.PhoneNumber.Trim()}", 60, 5, cancellationToken))
            throw new Exceptions.TooManyRequestsException("Too many OTP verification attempts for this phone number.");

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
