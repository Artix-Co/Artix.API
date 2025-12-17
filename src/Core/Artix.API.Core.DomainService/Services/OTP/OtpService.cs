namespace Artix.API.Core.DomainService.Services.OTP;

using System.Security.Claims;
using System.Text.Json;
using Contract.Features.OTPs.Commands;
using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.Infra.Identity;
using Contract.Primitives.Infra.Redis;
using Domain.Entities.OTP;
using Domain.Entities.OTP.Enums;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Contract.Features.OTPs.Queries;
using Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;
using Contract.Primitives.DomainServices.OTP;
using Contract.Primitives.DomainServices.OTP.Init;
using Contract.Primitives.DomainServices.OTP.Verify;
using Domain.Entities.User.Enums;

internal sealed class OtpService : IOtpService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ISessionStore _sessionStore;
    private readonly IOTPCommandRepository _otpCommandRepository;
    private readonly IOTPQueryRepository _otpQueryRepository;

    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    // private readonly ISmsSender _smsSender; 
    private readonly JsonSerializerOptions _jsonOptions;

    public OtpService(
        UserManager<AppUser> userManager,
        ISessionStore sessionStore,
        IOTPCommandRepository otpCommandRepository,
        IOTPQueryRepository otpQueryRepository,
        IJwtTokenGenerator jwtTokenGenerator
        // ISmsSender smsSender
    )
    {
        _userManager = userManager;
        _sessionStore = sessionStore;
        _otpCommandRepository = otpCommandRepository;
        _otpQueryRepository = otpQueryRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        // _smsSender = smsSender;

        _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    public async Task<InitOTPResult> InitAsync(InitOTPRequest request, CancellationToken cancellationToken = default)
    {
        var userExists = await _userManager.Users
            .AnyAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        var purpose = userExists ? PurposeType.Login : PurposeType.Registration;

        var otp = OTP.Generate(request.PhoneNumber, purpose);
        var businessId = otp.BusinessId;

        var smsMessage = $"Your {purpose.ToString().ToLower()} OTP is {otp.Code}. It expires in 5 minutes.";
        // await _smsSender.SendAsync(command.PhoneNumber, smsMessage, cancellationToken);

        var sessionData = new OtpSessionData(otp.Code, purpose, 0);
        var json = JsonSerializer.Serialize(sessionData, _jsonOptions);

        await _sessionStore.SetSessionAsync($"otp:{request.PhoneNumber}", json, 300, cancellationToken);

        await _otpCommandRepository.InsertAsync(otp, cancellationToken);

        return new InitOTPResult(businessId);
    }

    public async Task<VerifyOTPResult> VerifyAsync(VerifyOTPRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = $"otp:{request.PhoneNumber}";

        var json = await _sessionStore.GetSessionAsync(key, cancellationToken);
        if (json is null)
            throw new UnauthorizedAccessException("OTP expired or not found.");

        var data = JsonSerializer.Deserialize<OtpSessionData>(json, _jsonOptions)!;

        if (data.Attempts >= 3)
            throw new UnauthorizedAccessException("Too many failed attempts.");

        if (data.Code != request.OtpCode)
        {
            var updated = data with { Attempts = data.Attempts + 1 };
            await _sessionStore.SetSessionAsync(
                key,
                JsonSerializer.Serialize(updated, _jsonOptions),
                300,
                cancellationToken);

            throw new UnauthorizedAccessException("Invalid OTP.");
        }

        await _sessionStore.RemoveSessionAsync(key, cancellationToken);

        AppUser? user = null;

        if (data.Purpose == PurposeType.Registration)
        {
            user = await CreateClientUserAsync(request.PhoneNumber);
        }
        else
        {
            user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
        }

        if (user is null)
            throw new InvalidOperationException("Invalid OTP purpose or user state.");

        var tokens = await _jwtTokenGenerator.GenerateTokensAsync(user, true, cancellationToken);

        var latestByPhoneNumberDto = await _otpQueryRepository.GetLatestByPhoneNumberAsync(
            new GetLatestOTPByPhoneNumberQuery(request.PhoneNumber, request.OtpCode), cancellationToken);

        var otp = await _otpCommandRepository.GetByIdAsync(latestByPhoneNumberDto.Id, cancellationToken);

        if (otp == null)
            throw new ApplicationException($"Unable to get OTP for user {request.PhoneNumber}");

        otp.MarkAsUsed();
        await _otpCommandRepository.UpdateAsync(otp, cancellationToken);

        var result = new VerifyOTPResult(
            user.BusinessId,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt
        );
        return result;
    }

    private async Task<AppUser> CreateClientUserAsync(string phoneNumber)
    {
        var user = new AppUser
        {
            UserName = $"user_{phoneNumber}",
            Email = $"{phoneNumber}@example.com",
            PhoneNumber = phoneNumber,
            DisplayName = phoneNumber,
            PhoneNumberConfirmed = true
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to create user: " +
                                                string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, nameof(Role.Client));

        var claim = new Claim("ClientType", ClientType.Emerald.ToString());
        var addClaimResult = await _userManager.AddClaimAsync(user, claim);

        if (!addClaimResult.Succeeded)
            throw new InvalidOperationException("Failed to add ClientType claim: " +
                                                string.Join(", ", addClaimResult.Errors.Select(e => e.Description)));

        return user;
    }
}
