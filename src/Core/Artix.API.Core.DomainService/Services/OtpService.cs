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
    private readonly JsonSerializerOptions _jsonOptions;

    public OtpService(
        UserManager<AppUser> userManager,
        ISessionStore sessionStore,
        IOTPCommandRepository otpCommandRepository,
        IOTPQueryRepository otpQueryRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _sessionStore = sessionStore;
        _otpCommandRepository = otpCommandRepository;
        _otpQueryRepository = otpQueryRepository;
        _jwtTokenGenerator = jwtTokenGenerator;

        _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    public async Task<InitOTPResult> InitAsync(InitOTPRequest request, CancellationToken cancellationToken = default)
    {
        var purpose = await DetermineOtpPurposeAsync(request.PhoneNumber, cancellationToken);

        var otpEntity = OTP.Generate(request.PhoneNumber, purpose);
        var otpSession = new OtpSessionData(otpEntity.Code, purpose, 0);

        var sessionJson = SerializeOtpSession(otpSession);

        var sessionKey = GetOtpSessionKey(request.PhoneNumber);

        var insertOtpTask = _otpCommandRepository.InsertAsync(otpEntity, cancellationToken);
        var setSessionTask = _sessionStore.SetSessionAsync(sessionKey, sessionJson, ttlSeconds: 300, cancellationToken);

        await Task.WhenAll(insertOtpTask, setSessionTask);

        return new InitOTPResult(otpEntity.BusinessId);
    }

    public async Task<VerifyOTPResult> VerifyAsync(VerifyOTPRequest request,
        CancellationToken cancellationToken = default)
    {
        var sessionKey = GetOtpSessionKey(request.PhoneNumber);

        var otpSession = await GetOtpSessionAsync(sessionKey, cancellationToken);
        if (otpSession == null)
            throw new UnauthorizedAccessException("OTP expired or not found.");

        if (otpSession.Attempts >= MaxFailedOtpAttempts)
            throw new UnauthorizedAccessException("Too many failed attempts.");

        if (otpSession.Code != request.OtpCode)
        {
            await IncrementAndSaveFailedAttemptsAsync(sessionKey, otpSession, cancellationToken);
            throw new UnauthorizedAccessException("Invalid OTP.");
        }

        var removeSessionTask = _sessionStore.RemoveSessionAsync(sessionKey, cancellationToken);

        var user = await GetOrCreateUserAsync(request.PhoneNumber, otpSession.Purpose, otpSession.Attempts,
            cancellationToken);

        var generateTokensTask = _jwtTokenGenerator.GenerateTokensAsync(user, true, cancellationToken);

        var latestOtpDto = await _otpQueryRepository.GetLatestByPhoneNumberAsync(
            new GetLatestOTPByPhoneNumberQuery(request.PhoneNumber, request.OtpCode), cancellationToken);

        var otpEntity = await _otpCommandRepository.GetByIdAsync(latestOtpDto.Id, cancellationToken)
                        ?? throw new ApplicationException($"Unable to get OTP for user {request.PhoneNumber}");

        otpEntity.MarkAsUsed();
        var updateOtpTask = _otpCommandRepository.UpdateAsync(otpEntity, cancellationToken);

        await removeSessionTask;

        var tokens = await generateTokensTask;
        await updateOtpTask;

        return new VerifyOTPResult(
            user.BusinessId,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt);
    }

    private async Task<PurposeType> DetermineOtpPurposeAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.Users
            .AsNoTracking()
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        return userExists ? PurposeType.Login : PurposeType.Registration;
    }

    private async Task<AppUser> GetOrCreateUserAsync(string phoneNumber, PurposeType purpose, int failedAttempts,
        CancellationToken cancellationToken)
    {
        if (purpose == PurposeType.Registration)
        {
            return await CreateNewUserAsync(phoneNumber, failedAttempts, cancellationToken);
        }

        var existingUser = await _userManager.Users
                               .AsNoTracking()
                               .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken)
                           ?? throw new InvalidOperationException("Invalid OTP purpose or user state.");

        return existingUser;
    }

    private async Task<AppUser> CreateNewUserAsync(string phoneNumber, int failedAttempts,
        CancellationToken cancellationToken)
    {
        var shouldLockout = failedAttempts >= MaxFailedOtpAttempts;

        var newUser = new AppUser
        {
            UserName = $"user_{phoneNumber}",
            Email = $"{phoneNumber}@artix-studio.com",
            PhoneNumber = phoneNumber,
            DisplayName = "کاربر آرتیکس",
            PhoneNumberConfirmed = true,
            AccessFailedCount = failedAttempts,
            LockoutEnabled = true,
            LockoutEnd = shouldLockout ? DateTimeOffset.UtcNow.Add(LockoutDuration) : null,
            TwoFactorEnabled = false,
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
            throw new InvalidOperationException("Failed to create user: " +
                                                string.Join(", ", createResult.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(newUser, nameof(Role.Client));

        var claimResult =
            await _userManager.AddClaimAsync(newUser, new Claim("ClientType", ClientType.Emerald.ToString()));
        if (!claimResult.Succeeded)
            throw new InvalidOperationException("Failed to add ClientType claim: " +
                                                string.Join(", ", claimResult.Errors.Select(e => e.Description)));

        return newUser;
    }

    private string GetOtpSessionKey(string phoneNumber) => $"otp:{phoneNumber}";

    private string SerializeOtpSession(OtpSessionData otpSession) => JsonSerializer.Serialize(otpSession, _jsonOptions);

    private OtpSessionData? DeserializeOtpSession(string? sessionJson)
    {
        return sessionJson == null ? null : JsonSerializer.Deserialize<OtpSessionData>(sessionJson, _jsonOptions);
    }

    private async Task<OtpSessionData?> GetOtpSessionAsync(string sessionKey, CancellationToken cancellationToken)
    {
        var sessionJson = await _sessionStore.GetSessionAsync(sessionKey, cancellationToken);
        return DeserializeOtpSession(sessionJson);
    }

    private async Task IncrementAndSaveFailedAttemptsAsync(string sessionKey, OtpSessionData otpSession,
        CancellationToken cancellationToken)
    {
        var updatedOtpSession = otpSession with { Attempts = otpSession.Attempts + 1 };
        var updatedJson = SerializeOtpSession(updatedOtpSession);

        await _sessionStore.SetSessionAsync(sessionKey, updatedJson, ttlSeconds: 300, cancellationToken);
    }
}
