namespace Artix.API.Core.ApplicationService.Features.Users.Queries.VerifyOTPAuth;

using System.Security.Claims;
using Primitives;
using Domain.Entities.User;
using Contract.Features.Users.Queries.VerifyOTPAuth;
using Contract.Primitives.Infra.Redis;
using Contract.Primitives.Models;
using Domain.Entities.User.Enums;
using Infra.Identity.Interfaces.LoginHistory;
using Infra.Identity.Interfaces.TokenProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// TODO: develop validator for this handler
internal sealed class VerifyOTPAuthHandler : QueryHandlerBase<GetVerifyOTPAuthQuery, VerifyOTPAuthDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserLoginHistoryService _userLoginHistoryService;
    private readonly ISessionStore _sessionStore;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;


    public VerifyOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IUserLoginHistoryService userLoginHistoryService, ISessionStore sessionStore, IJwtTokenGenerator jwtTokenGenerator) : base(httpContextAccessor, userManager)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._signInManager = signInManager;
        this._userLoginHistoryService = userLoginHistoryService;
        this._sessionStore = sessionStore;
        this._jwtTokenGenerator = jwtTokenGenerator;
    }

    public override async Task<Result<VerifyOTPAuthDto>> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        var json = await _sessionStore.GetSessionAsync($"otp:{query.PhoneNumber}", cancellationToken);
        if (string.IsNullOrEmpty(json))
            throw new UnauthorizedAccessException("OTP expired or not found.");

        var data = System.Text.Json.JsonSerializer.Deserialize<OtpSessionData>(json)!;
        if (data.Attempts >= 3)
            throw new UnauthorizedAccessException("Too many failed attempts.");
        if (data.Code != query.OtpCode)
        {
            await _sessionStore.SetSessionAsync(
                $"otp:{query.PhoneNumber}",
                System.Text.Json.JsonSerializer.Serialize(data with { Attempts = data.Attempts + 1 }),
                300,
                cancellationToken);
            throw new UnauthorizedAccessException("Invalid OTP.");
        }

        await _sessionStore.RemoveSessionAsync($"otp:{query.PhoneNumber}", cancellationToken);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == query.PhoneNumber, cancellationToken);

        if (data.Purpose == "Registration" && user == null)
        {
            await EnsureClientRoleAsync();
            user = await CreateClientUserAsync(query.PhoneNumber);

            await _userLoginHistoryService.RecordLoginAsync(
                user,
                GetRemoteIp(),
                GetUserAgent());

            var tokens = await _jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: true, cancellationToken);

            return Result<VerifyOTPAuthDto>.Success(new VerifyOTPAuthDto(
                IsNewUser: true,
                UserId: user.BusinessId,
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken,
                AccessTokenExpiresAt: tokens.AccessTokenExpiresAt,
                RefreshTokenExpiresAt: tokens.RefreshTokenExpiresAt));
        }

        if (data.Purpose == "Login" && user != null)
        {
            await ValidateClientAccessAsync(user);

            await _signInManager.SignInAsync(user, isPersistent: false);
            await _userLoginHistoryService.RecordLoginAsync(user, GetRemoteIp(), GetUserAgent());

            var tokens = await _jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: true, cancellationToken);

            return Result<VerifyOTPAuthDto>.Success(new VerifyOTPAuthDto(
                IsNewUser: false,
                UserId: user.BusinessId,
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken,
                AccessTokenExpiresAt: tokens.AccessTokenExpiresAt,
                RefreshTokenExpiresAt: tokens.RefreshTokenExpiresAt));
        }

        throw new InvalidOperationException("Invalid OTP purpose or user state.");
    }


    private async Task<AppUser> CreateClientUserAsync(string phoneNumber)
    {
        var user = new AppUser
        {
            UserName = $"user_{phoneNumber}",
            Email = $"{phoneNumber}@example.com",
            PhoneNumber = phoneNumber,
            DisplayName = phoneNumber
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            throw new ApplicationException("User creation failed.");

        var roleResult = await _userManager.AddToRoleAsync(user, nameof(Role.Client));
        if (!roleResult.Succeeded)
            throw new ApplicationException("Role assignment failed.");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.DisplayName ?? user.UserName),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim("ClientType", nameof(ClientType.Emerald)), new Claim("permission", "read:client"),
            new Claim("permission", "view:profile"), new Claim("permission_group", "client_group"),
            new Claim("group", "client_team"), new Claim("group_permission", "client_team_read")
        };

        var claimResult = await _userManager.AddClaimsAsync(user, claims);
        if (!claimResult.Succeeded)
            throw new ApplicationException("Claim assignment failed.");

        return user;
    }
    protected async Task EnsureClientRoleAsync()
    {
        if (!await _roleManager.RoleExistsAsync(nameof(Role.Client)))
        {
            var result = await _roleManager.CreateAsync(new AppRole(nameof(Role.Client)));
            if (!result.Succeeded)
                throw new ApplicationException("Failed to create Client role.");
        }
    }
}
