namespace Artix.API.Core.ApplicationService.Features.Users.Client.Commands.InitiateOTPAuth;

using System.Text.Json;
using Primitives;
using Artix.API.Core.Contract.Features.OTPs.Commands;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Contract.Features.Users.Client.Commands.InitiateOTPAuth;
using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Domain.Entities.OTP;
using Domain.Entities.OTP.Enums;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// TODO: develop validation for this handler
internal sealed class InitiateOTPAuthHandler : CommandHandlerBase<InitiateOTPAuthCommand>
{
    private readonly ISessionStore _sessionStore;
    private readonly IOTPCommandRepository _otpCommandRepository;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public InitiateOTPAuthHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<InitiateOTPAuthCommand>> logger,
        ISessionStore sessionStore,
        IOTPCommandRepository otpCommandRepository) : base(httpContextAccessor, userManager, logger)
    {
        this._sessionStore = sessionStore;
        this._otpCommandRepository = otpCommandRepository;
    }

    public override async Task<Guid> Handle(InitiateOTPAuthCommand command, CancellationToken cancellationToken)
    {
        var userExists = await this._userManager.Users
            .AnyAsync(u => u.PhoneNumber == command.PhoneNumber, cancellationToken);

        var purpose = userExists ? PurposeType.Login : PurposeType.Registration;
        var otp = OTP.Generate(command.PhoneNumber, purpose);
        var businessId = otp.BusinessId;

        var smsMessage = $"Your {purpose.ToString().ToLower()} OTP is {otp.Code}. It expires in 5 minutes.";
        // await _smsSender.SendAsync(command.PhoneNumber, smsMessage, cancellationToken);


        var sessionData = new OtpSessionData(otp.Code, purpose, 0);

        var json = JsonSerializer.Serialize(sessionData, _jsonOptions);
        await this._sessionStore.SetSessionAsync($"otp:{command.PhoneNumber}", json, 300, cancellationToken);

        
        await this._otpCommandRepository.InsertAsync(otp, cancellationToken);

        return businessId;
    }
}
