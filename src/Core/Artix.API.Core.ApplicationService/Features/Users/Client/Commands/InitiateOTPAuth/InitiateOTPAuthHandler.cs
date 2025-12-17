namespace Artix.API.Core.ApplicationService.Features.Users.Client.Commands.InitiateOTPAuth;

using Primitives;
using Contract.Features.Users.Client.Commands.InitiateOTPAuth;
using Contract.Primitives.DomainServices.OTP;
using Contract.Primitives.DomainServices.OTP.Init;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// TODO: develop validation for this handler
internal sealed class InitiateOTPAuthHandler : CommandHandlerBase<InitiateOTPAuthCommand>
{
    private readonly IOtpService _otpService;

    public InitiateOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<InitiateOTPAuthCommand>> logger, IOtpService otpService) : base(httpContextAccessor,
        userManager, logger)
    {
        this._otpService = otpService;
    }

    public override async Task<Guid> Handle(InitiateOTPAuthCommand command, CancellationToken cancellationToken)
    {
        var initOtpResult =
            await this._otpService.InitAsync(new InitOTPRequest(command.PhoneNumber), cancellationToken);


        return initOtpResult.Id;
    }
}
