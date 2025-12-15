namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth;

using System.Security.Claims;
using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.Infra.Identity;
using Contract.Primitives.Infra.Redis;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetVerifyOTPAuthHandler : QueryHandlerBase<GetVerifyOTPAuthQuery, VerifyOTPAuthDto>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ISessionStore _sessionStore;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;


    public GetVerifyOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager,
        ISessionStore sessionStore,
        IJwtTokenGenerator jwtTokenGenerator) : base(httpContextAccessor, userManager)
    {
        this._roleManager = roleManager;
        this._signInManager = signInManager;
        this._sessionStore = sessionStore;
        this._jwtTokenGenerator = jwtTokenGenerator;
    }

    public override async Task<Result<VerifyOTPAuthDto>> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        var json = await this._sessionStore.GetSessionAsync($"otp:{query.PhoneNumber}", cancellationToken);
        if (string.IsNullOrEmpty(json))
            throw new UnauthorizedAccessException("OTP expired or not found.");

        var data = System.Text.Json.JsonSerializer.Deserialize<OtpSessionData>(json)!;
        if (data.Attempts >= 3)
            throw new UnauthorizedAccessException("Too many failed attempts.");
        if (data.Code != query.OtpCode)
        {
            await this._sessionStore.SetSessionAsync(
                $"otp:{query.PhoneNumber}",
                System.Text.Json.JsonSerializer.Serialize(data with { Attempts = data.Attempts + 1 }),
                300,
                cancellationToken);
            throw new UnauthorizedAccessException("Invalid OTP.");
        }

        await this._sessionStore.RemoveSessionAsync($"otp:{query.PhoneNumber}", cancellationToken);

        var user = await this._userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == query.PhoneNumber, cancellationToken);

        if (data.Purpose == "Registration" && user == null)
        {
            await this.EnsureClientRoleAsync();
            user = await this.CreateClientUserAsync(query.PhoneNumber);


            var tokens =
                await this._jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: true, cancellationToken);

            return Result<VerifyOTPAuthDto>.Success(new VerifyOTPAuthDto(
                UserId: user.BusinessId,
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken,
                AccessTokenExpiresAt: tokens.AccessTokenExpiresAt,
                RefreshTokenExpiresAt: tokens.RefreshTokenExpiresAt));
        }

        if (data.Purpose == "Login" && user != null)
        {
            await this.ValidateClientAccessAsync(user);

            await this._signInManager.SignInAsync(user, isPersistent: false);

            var tokens =
                await this._jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: true, cancellationToken);

            return Result<VerifyOTPAuthDto>.Success(new VerifyOTPAuthDto(
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

        var createResult = await this._userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            throw new ApplicationException("User creation failed.");

        var roleResult = await this._userManager.AddToRoleAsync(user, nameof(Role.Client));
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

        var claimResult = await this._userManager.AddClaimsAsync(user, claims);
        if (!claimResult.Succeeded)
            throw new ApplicationException("Claim assignment failed.");

        return user;
    }

    private async Task EnsureClientRoleAsync()
    {
        if (!await this._roleManager.RoleExistsAsync(nameof(Role.Client)))
        {
            var result = await this._roleManager.CreateAsync(new AppRole(nameof(Role.Client)));
            if (!result.Succeeded)
                throw new ApplicationException("Failed to create Client role.");
        }
    }
}
