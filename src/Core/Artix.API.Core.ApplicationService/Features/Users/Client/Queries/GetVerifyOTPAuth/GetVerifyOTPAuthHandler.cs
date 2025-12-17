namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth;

using Contract.Features.OTPs.Commands;
using Contract.Features.OTPs.Queries;
using Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;
using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.Infra.Identity.Authentication;
using Contract.Primitives.Infra.Identity.Authentication.Client.Login;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetVerifyOTPAuthHandler : QueryHandlerBase<GetVerifyOTPAuthQuery, VerifyOTPAuthDto>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IOTPQueryRepository _otpQueryRepository;
    private readonly IOTPCommandRepository _otpCommandRepository;

    public GetVerifyOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IAuthenticationService authenticationService, IOTPQueryRepository otpQueryRepository,
        IOTPCommandRepository otpCommandRepository) : base(
        httpContextAccessor, userManager)
    {
        this._authenticationService = authenticationService;
        this._otpQueryRepository = otpQueryRepository;
        this._otpCommandRepository = otpCommandRepository;
    }

    public override async Task<Result<VerifyOTPAuthDto>> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        var authenticationResult =
            await this._authenticationService.ClientOtpLoginAsync(
                new ClientLoginRequest(query.PhoneNumber, query.OtpCode), cancellationToken);

        var latestByPhoneNumberDto =
            await this._otpQueryRepository.GetLatestByPhoneNumberAsync(
                new GetLatestOTPByPhoneNumberQuery(query.PhoneNumber, query.OtpCode), cancellationToken);
        
        var otp = await this._otpCommandRepository.GetByIdAsync(latestByPhoneNumberDto.Id, cancellationToken);
        
        if (otp == null)
        {
            throw new ApplicationException($"Unable to get OTP for user {query.PhoneNumber}");
        }

        otp.MarkAsUsed();
        await _otpCommandRepository.UpdateAsync(otp, cancellationToken);

        var result = new VerifyOTPAuthDto(
            UserId: authenticationResult.UserId,
            AccessToken: authenticationResult.AccessToken,
            RefreshToken: authenticationResult.RefreshToken,
            AccessTokenExpiresAt: authenticationResult.AccessTokenExpiresAt,
            RefreshTokenExpiresAt: authenticationResult.RefreshTokenExpiresAt);


        return Result<VerifyOTPAuthDto>.Success(result);
    }
}
