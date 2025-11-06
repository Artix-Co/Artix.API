namespace Artix.API.Core.ApplicationService.Features.Users.Commands.InitiateOTPAuth;

using Contract.Features.OTPs.Commands;
using Contract.Features.Users.Commands.InitiateOTPAuth;
using Domain.Entities.OTP;
using Domain.Entities.User;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Primitives;

// TODO: develop validation for this handler
internal sealed class InitiateOTPAuthHandler : CommandHandlerBase<InitiateOTPAuthCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ISessionStore _sessionStore;
    private readonly IOTPCommandRepository _otpCommandRepository;

    public InitiateOTPAuthHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ISessionStore sessionStore,
        IOTPCommandRepository otpCommandRepository) : base(httpContextAccessor, userManager)
    {
        _userManager = userManager;
        _sessionStore = sessionStore;
        _otpCommandRepository = otpCommandRepository;
    }

    public override async Task<Guid> Handle(InitiateOTPAuthCommand command, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.Users
            .AnyAsync(u => u.PhoneNumber == command.PhoneNumber, cancellationToken);

        var purpose = userExists ? "Login" : "Registration";
        var otp = OTP.Create(command.PhoneNumber, purpose);
        var businessId = otp.BusinessId;

        var smsMessage = $"Your {purpose.ToLower()} OTP is {otp.Code}. It expires in 5 minutes.";
        // await _smsSender.SendAsync(command.PhoneNumber, smsMessage, cancellationToken);

        var sessionData = new { Code = otp.Code, Purpose = purpose, Attempts = 0 };
        var json = System.Text.Json.JsonSerializer.Serialize(sessionData);
        await _sessionStore.SetSessionAsync($"otp:{command.PhoneNumber}", json, 300, cancellationToken);

        await _otpCommandRepository.InsertAsync(otp, cancellationToken);

        return businessId;
    }
}
