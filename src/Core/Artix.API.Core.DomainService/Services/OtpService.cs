namespace Artix.API.Core.DomainService.Services;

using System.Security.Claims;
using System.Text.Json;
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
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ISessionStore _sessionStore;
    private readonly IOTPCommandRepository _otpCommandRepository;
    private readonly IOTPQueryRepository _otpQueryRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly JsonSerializerOptions _jsonOptions;

    public OtpService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ISessionStore sessionStore,
        IOTPCommandRepository otpCommandRepository,
        IOTPQueryRepository otpQueryRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _sessionStore = sessionStore;
        _otpCommandRepository = otpCommandRepository;
        _otpQueryRepository = otpQueryRepository;
        _jwtTokenGenerator = jwtTokenGenerator;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<InitOTPResult> InitAsync(
        InitOTPRequest request,
        CancellationToken cancellationToken = default)
    {
        var purpose = await DetermineOtpPurposeAsync(request.PhoneNumber, cancellationToken);

        var otpEntity = OTP.Generate(request.PhoneNumber, purpose);
        var otpSession = new OtpSessionData(otpEntity.Code, purpose, 0);

        var sessionKey = GetOtpSessionKey(request.PhoneNumber);
        var sessionJson = JsonSerializer.Serialize(otpSession, _jsonOptions);

        await _otpCommandRepository.InsertAsync(otpEntity, cancellationToken);
        await _sessionStore.SetSessionAsync(sessionKey, sessionJson, ttlSeconds: 300, cancellationToken);

        return new InitOTPResult(otpEntity.BusinessId);
    }

    public async Task<VerifyOTPResult> VerifyAsync(
        VerifyOTPRequest request,
        CancellationToken cancellationToken = default)
    {
        var sessionKey = GetOtpSessionKey(request.PhoneNumber);
        var otpSession = await GetOtpSessionAsync(sessionKey, cancellationToken);

        if (otpSession is null)
            throw new UnauthorizedAccessException("OTP expired or not found.");

        if (otpSession.Attempts >= MaxFailedOtpAttempts)
            throw new UnauthorizedAccessException("Too many failed attempts.");

        if (otpSession.Code != request.OtpCode)
        {
            await IncrementFailedAttemptsAsync(sessionKey, otpSession, cancellationToken);
            throw new UnauthorizedAccessException("Invalid OTP.");
        }

        var user = await GetOrCreateUserAsync(
            request.PhoneNumber,
            otpSession.Purpose,
            otpSession.Attempts,
            cancellationToken);

        var tokens = await _jwtTokenGenerator.GenerateTokensAsync(user, true, cancellationToken);

        var latestOtp = await _otpQueryRepository.GetLatestByPhoneNumberAsync(
            new GetLatestOTPByPhoneNumberQuery(request.PhoneNumber, request.OtpCode),
            cancellationToken);

        var otpEntity = await _otpCommandRepository.GetByIdAsync(latestOtp.Id, cancellationToken)
                        ?? throw new InvalidOperationException("OTP not found.");

        otpEntity.MarkAsUsed();
        await _otpCommandRepository.UpdateAsync(otpEntity, cancellationToken);

        await _sessionStore.RemoveSessionAsync(sessionKey, cancellationToken);

        return new VerifyOTPResult(
            user.BusinessId,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt);
    }

    private async Task<PurposeType> DetermineOtpPurposeAsync(
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        var exists = await _userManager.Users
            .AsNoTracking()
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

        return exists ? PurposeType.Login : PurposeType.Registration;
    }

    private async Task<AppUser> GetOrCreateUserAsync(
        string phoneNumber,
        PurposeType purpose,
        int failedAttempts,
        CancellationToken cancellationToken)
    {
        if (purpose == PurposeType.Registration)
            return await CreateNewUserAsync(phoneNumber, failedAttempts, cancellationToken);

        return await _userManager.Users
                   .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken)
               ?? throw new InvalidOperationException("User not found.");
    }

    private async Task<AppUser> CreateNewUserAsync(
        string phoneNumber,
        int failedAttempts,
        CancellationToken cancellationToken)
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
                : null
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        await EnsureRoleExistsAsync(nameof(Role.Client));

        var roleResult = await _userManager.AddToRoleAsync(user, nameof(Role.Client));
        if (!roleResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));

        var claimResult = await _userManager.AddClaimAsync(
            user,
            new Claim("ClientType", ClientType.Emerald.ToString()));

        if (!claimResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", claimResult.Errors.Select(e => e.Description)));

        return user;
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return;

        var result = await _roleManager.CreateAsync(new AppRole(roleName));
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    private static string GetOtpSessionKey(string phoneNumber) => $"otp:{phoneNumber}";

    private async Task<OtpSessionData?> GetOtpSessionAsync(
        string sessionKey,
        CancellationToken cancellationToken)
    {
        var json = await _sessionStore.GetSessionAsync(sessionKey, cancellationToken);
        return json == null
            ? null
            : JsonSerializer.Deserialize<OtpSessionData>(json, _jsonOptions);
    }

    private async Task IncrementFailedAttemptsAsync(
        string sessionKey,
        OtpSessionData session,
        CancellationToken cancellationToken)
    {
        var updated = session with { Attempts = session.Attempts + 1 };
        var json = JsonSerializer.Serialize(updated, _jsonOptions);

        await _sessionStore.SetSessionAsync(sessionKey, json, ttlSeconds: 300, cancellationToken);
    }
}
