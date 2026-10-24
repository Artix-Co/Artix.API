namespace Artix.API.Core.ApplicationService.Features.Users.Client.Commands.InitiateOTPAuth;

using Primitives;
using Contract.Features.Users.Client.Commands.InitiateOTPAuth;
using Contract.Primitives.DomainServices.OTP;
using Contract.Primitives.DomainServices.OTP.Init;
using Contract.Primitives.Infra.Redis;
using Exceptions;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

internal sealed class InitiateOTPAuthHandler : CommandHandlerBase<InitiateOTPAuthCommand>
{
    private readonly IOtpService _otpService;
    private readonly IRequestRatePolicy _ratePolicy;

    public InitiateOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<InitiateOTPAuthCommand>> logger, IOtpService otpService, IRequestRatePolicy ratePolicy) : base(httpContextAccessor,
        userManager, logger)
    {
        this._otpService = otpService;
        _ratePolicy = ratePolicy;
    }

    public override async Task<Guid> Handle(InitiateOTPAuthCommand command, CancellationToken cancellationToken)
    {
        if (!await _ratePolicy.IsAllowedAsync($"otp:send:phone:{command.PhoneNumber.Trim()}", 120, 2, cancellationToken))
            throw new Exceptions.TooManyRequestsException("Too many OTP requests for this phone number.");

        var initOtpResult =
            await this._otpService.InitAsync(new InitOTPRequest(command.PhoneNumber), cancellationToken);


        return initOtpResult.Id;
    }
}
