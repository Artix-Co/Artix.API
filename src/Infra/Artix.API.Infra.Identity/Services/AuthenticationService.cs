namespace Artix.API.Infra.Identity.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Core.Contract.Primitives.Infra.Identity;
using Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Login;
using Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Logout;
using Core.Contract.Primitives.Infra.Identity.Authentication.Client.Login;
using Core.Contract.Primitives.Infra.Identity.Authentication.Client.Logout;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.Entities.User;
using Core.Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IAuthenticationService = Core.Contract.Primitives.Infra.Identity.Authentication.IAuthenticationService;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ISessionStore _sessionStore;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITokenRevocationStore _revocationStore;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        SignInManager<AppUser> signInManager,
        ISessionStore sessionStore,
        IJwtTokenGenerator jwtTokenGenerator,
        ITokenRevocationStore revocationStore,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _sessionStore = sessionStore;
        _jwtTokenGenerator = jwtTokenGenerator;
        _revocationStore = revocationStore;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ClientLoginResponse> ClientOtpLoginAsync(ClientLoginRequest request, CancellationToken cancellationToken = default)
    {
        var json = await _sessionStore.GetSessionAsync($"otp:{request.PhoneNumber}", cancellationToken);
        if (string.IsNullOrEmpty(json))
            throw new UnauthorizedAccessException("OTP expired or not found.");

        var data = JsonSerializer.Deserialize<OtpSessionData>(json)!;

        if (data.Attempts >= 3)
            throw new UnauthorizedAccessException("Too many failed attempts.");

        if (data.Code != request.OtpCode)
        {
            await _sessionStore.SetSessionAsync(
                $"otp:{request.PhoneNumber}",
                JsonSerializer.Serialize(data with { Attempts = data.Attempts + 1 }),
                300,
                cancellationToken);

            throw new UnauthorizedAccessException("Invalid OTP.");
        }

        await _sessionStore.RemoveSessionAsync($"otp:{request.PhoneNumber}", cancellationToken);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (data.Purpose == "Registration" && user == null)
            user = await CreateClientUserAsync(request.PhoneNumber);

        if (data.Purpose == "Login" && user != null)
            await _signInManager.SignInAsync(user, isPersistent: false);

        if (user == null)
            throw new InvalidOperationException("Invalid OTP purpose or user state.");

        var tokens = await _jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: true, cancellationToken);

        return new ClientLoginResponse(
            user.BusinessId,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt);
    }

    public async Task<ClientLogoutResponse> ClientLogoutAsync(ClientLogoutRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        await PerformLogoutAsync(user, cancellationToken);
        return new ClientLogoutResponse();
    }

    public async Task<AdminLoginResponse> AdminLoginAsync(AdminLoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(nameof(Role.Admin)))
            throw new UnauthorizedAccessException("Access denied: Admin role required.");

        var tokens = await _jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: true, cancellationToken);

        return new AdminLoginResponse(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt,
            user.UserName!,
            user.DisplayName,
            roles.ToArray().AsReadOnly());
    }

    public async Task<AdminLogoutResponse> AdminLogoutAsync(AdminLogoutRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        await PerformLogoutAsync(user, cancellationToken);
        return new AdminLogoutResponse();
    }

    private async Task<AppUser> GetCurrentUserAsync(CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("User not authenticated.");

        var user = await _userManager.FindByIdAsync(userId);
        return user ?? throw new UnauthorizedAccessException("User not found.");
    }

    private async Task PerformLogoutAsync(AppUser user, CancellationToken ct)
    {
        var accessToken = await _userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jti != null)
            {
                var expiry = DateTimeOffset.FromUnixTimeSeconds(jwt.ValidTo.ToUniversalTime().Ticks / TimeSpan.TicksPerSecond);
                await _revocationStore.RevokeAsync(jti, expiry);
            }
        }

        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");
        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");

    }

    private async Task<AppUser> CreateClientUserAsync(string phoneNumber)
    {
        await EnsureClientRoleExistsAsync();

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

        await _userManager.AddToRoleAsync(user, nameof(Role.Client));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.DisplayName ?? user.UserName),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim("ClientType", nameof(ClientType.Emerald)),
            new Claim("permission", "read:client"),
            new Claim("permission", "view:profile"),
            new Claim("permission_group", "client_group"),
            new Claim("group", "client_team"),
            new Claim("group_permission", "client_team_read")
        };

        await _userManager.AddClaimsAsync(user, claims);

        return user;
    }

    private async Task EnsureClientRoleExistsAsync()
    {
        if (!await _roleManager.RoleExistsAsync(nameof(Role.Client)))
            await _roleManager.CreateAsync(new AppRole(nameof(Role.Client)));
    }
}
