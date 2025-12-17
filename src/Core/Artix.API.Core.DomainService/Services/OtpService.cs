namespace Artix.API.Core.DomainService.Services;

using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using Contract.Features.OTPs.Commands;
using Contract.Features.OTPs.Queries;
using Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;
using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.DomainServices.OTP;
using Contract.Primitives.DomainServices.OTP.Init;
using Contract.Primitives.DomainServices.OTP.Verify;
using Contract.Primitives.Infra.Identity;
using Contract.Primitives.Infra.Redis;
using Domain.Entities.OTP;
using Domain.Entities.OTP.Enums;
using Domain.Entities.User;
using Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal sealed class OtpService : IOtpService
{
    private const int MaxFailedOtpAttempts = 3;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromHours(1);


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
        this._userManager = userManager;
        this._sessionStore = sessionStore;
        this._otpCommandRepository = otpCommandRepository;
        this._otpQueryRepository = otpQueryRepository;
        this._jwtTokenGenerator = jwtTokenGenerator;
        // _smsSender = smsSender;

        this._jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    public async Task<InitOTPResult> InitAsync(InitOTPRequest request, CancellationToken cancellationToken = default)
    {
        var userExists = await this._userManager.Users
            .AnyAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        var purpose = userExists ? PurposeType.Login : PurposeType.Registration;

        var otp = OTP.Generate(request.PhoneNumber, purpose);
        var businessId = otp.BusinessId;

        var smsMessage = $"Your {purpose.ToString().ToLower()} OTP is {otp.Code}. It expires in 5 minutes.";
        // await _smsSender.SendAsync(command.PhoneNumber, smsMessage, cancellationToken);

        var sessionData = new OtpSessionData(otp.Code, purpose, 0);
        var json = JsonSerializer.Serialize(sessionData, this._jsonOptions);

        await this._sessionStore.SetSessionAsync($"otp:{request.PhoneNumber}", json, 300, cancellationToken);

        await this._otpCommandRepository.InsertAsync(otp, cancellationToken);

        return new InitOTPResult(businessId);
    }

    public async Task<VerifyOTPResult> VerifyAsync(VerifyOTPRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = $"otp:{request.PhoneNumber}";

        var json = await this._sessionStore.GetSessionAsync(key, cancellationToken);
        if (json is null)
            throw new UnauthorizedAccessException("OTP expired or not found.");

        var data = JsonSerializer.Deserialize<OtpSessionData>(json, this._jsonOptions)!;

        if (data.Attempts >= MaxFailedOtpAttempts)
            throw new UnauthorizedAccessException("Too many failed attempts.");

        if (data.Code != request.OtpCode)
        {
            var updated = data with { Attempts = data.Attempts + 1 };
            await this._sessionStore.SetSessionAsync(
                key,
                JsonSerializer.Serialize(updated, this._jsonOptions),
                300,
                cancellationToken);

            throw new UnauthorizedAccessException("Invalid OTP.");
        }

        await this._sessionStore.RemoveSessionAsync(key, cancellationToken);

        AppUser? user = null;

        if (data.Purpose == PurposeType.Registration)
        {
            user = await this.CreateClientUserAsync(request.PhoneNumber, data.Attempts);
        }
        else
        {
            user = await this._userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
        }

        if (user is null)
            throw new InvalidOperationException("Invalid OTP purpose or user state.");

        var tokens = await this._jwtTokenGenerator.GenerateTokensAsync(user, true, cancellationToken);

        var latestByPhoneNumberDto = await this._otpQueryRepository.GetLatestByPhoneNumberAsync(
            new GetLatestOTPByPhoneNumberQuery(request.PhoneNumber, request.OtpCode), cancellationToken);

        var otp = await this._otpCommandRepository.GetByIdAsync(latestByPhoneNumberDto.Id, cancellationToken);

        if (otp == null)
            throw new ApplicationException($"Unable to get OTP for user {request.PhoneNumber}");

        otp.MarkAsUsed();
        await this._otpCommandRepository.UpdateAsync(otp, cancellationToken);

        var result = new VerifyOTPResult(
            user.BusinessId,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt
        );
        return result;
    }

    private async Task<AppUser> CreateClientUserAsync(string phoneNumber, int failedAttempts)
    {
        var shouldLockout = failedAttempts >= MaxFailedOtpAttempts;

        var user = new AppUser
        {
            UserName = $"user_{phoneNumber}",
            Email = $"{phoneNumber}@artix-studio.com",
            PhoneNumber = phoneNumber,
            DisplayName = "کاربر آرتیکس",
            PhoneNumberConfirmed = true,
            AccessFailedCount = failedAttempts,
            LockoutEnabled = true,
            LockoutEnd = shouldLockout
                ? DateTimeOffset.UtcNow.Add(LockoutDuration)
                : null,
            TwoFactorEnabled = false,
        };

        var result = await this._userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to create user: " +
                                                string.Join(", ", result.Errors.Select(e => e.Description)));

        await this._userManager.AddToRoleAsync(user, nameof(Role.Client));

        var claim = new Claim("ClientType", ClientType.Emerald.ToString());
        var addClaimResult = await this._userManager.AddClaimAsync(user, claim);

        if (!addClaimResult.Succeeded)
            throw new InvalidOperationException("Failed to add ClientType claim: " +
                                                string.Join(", ", addClaimResult.Errors.Select(e => e.Description)));

        return user;
    }
}
