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
    private readonly ISessionStore _sessionStore;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserSessionService _userSessionService;
    private readonly ITokenRevocationStore _tokenRevocationStore;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ISessionStore sessionStore,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserSessionService userSessionService,
        ITokenRevocationStore tokenRevocationStore,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _sessionStore = sessionStore;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userSessionService = userSessionService;
        _tokenRevocationStore = tokenRevocationStore;
        _httpContextAccessor = httpContextAccessor;
    }

    // ---------------- LOGIN (OTP / CLIENT) ----------------

    public async Task<ClientLoginResponse> ClientOtpLoginAsync(
        ClientLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = await _sessionStore.GetSessionAsync(
            $"otp:{request.PhoneNumber}", cancellationToken);

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

        await _sessionStore.RemoveSessionAsync(
            $"otp:{request.PhoneNumber}", cancellationToken);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (data.Purpose == "Registration" && user == null)
            user = await CreateClientUserAsync(request.PhoneNumber);

        if (user == null)
            throw new InvalidOperationException("Invalid OTP purpose or user state.");

        var tokenResult = await _jwtTokenGenerator.GenerateTokensAsync(
            user, forceRefreshToken: true, cancellationToken);

        return new ClientLoginResponse(
            user.BusinessId,
            tokenResult.AccessToken,
            tokenResult.RefreshToken,
            tokenResult.AccessTokenExpiresAt,
            tokenResult.RefreshTokenExpiresAt);
    }

    // ---------------- LOGOUT (CLIENT + ADMIN) ----------------

    public async Task<ClientLogoutResponse> ClientLogoutAsync(
        ClientLogoutRequest request,
        CancellationToken cancellationToken = default)
    {
        await LogoutInternalAsync(cancellationToken);
        return new ClientLogoutResponse();
    }

    public async Task<AdminLogoutResponse> AdminLogoutAsync(
        AdminLogoutRequest request,
        CancellationToken cancellationToken = default)
    {
        await LogoutInternalAsync(cancellationToken);
        return new AdminLogoutResponse();
    }

    // ---------------- INTERNAL LOGOUT CORE ----------------

    private async Task LogoutInternalAsync(CancellationToken ct)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("No active HTTP context.");

        var user = await GetCurrentUserAsync(ct);

        // 1. Revoke ALL server-side sessions / refresh tokens
        await _userSessionService.RevokeAllAsync(user.Id, ct);

        // 2. Revoke CURRENT access token (JTI-based)
        var jti = httpContext.User
            .FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        var expClaim = httpContext.User
            .FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (!string.IsNullOrWhiteSpace(jti) && long.TryParse(expClaim, out var exp))
        {
            var expiry = DateTimeOffset.FromUnixTimeSeconds(exp);
            await _tokenRevocationStore.RevokeAsync(jti, expiry);
        }
    }

    // ---------------- ADMIN LOGIN ----------------

    public async Task<AdminLoginResponse> AdminLoginAsync(
        AdminLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);

        if (!roles.Contains(nameof(Role.Admin)))
            throw new UnauthorizedAccessException("Admin role required.");

        var tokenResult = await _jwtTokenGenerator.GenerateTokensAsync(
            user, forceRefreshToken: true, cancellationToken);

        return new AdminLoginResponse(
            tokenResult.AccessToken,
            tokenResult.RefreshToken,
            tokenResult.AccessTokenExpiresAt,
            tokenResult.RefreshTokenExpiresAt,
            user.UserName!,
            user.DisplayName,
            roles.ToArray().AsReadOnly());
    }

    // ---------------- HELPERS ----------------

    private async Task<AppUser> GetCurrentUserAsync(CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        return await _userManager.FindByIdAsync(userId)
               ?? throw new UnauthorizedAccessException("User not found.");
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

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new ApplicationException("User creation failed.");

        await _userManager.AddToRoleAsync(user, nameof(Role.Client));
        return user;
    }

    private async Task EnsureClientRoleExistsAsync()
    {
        if (!await _roleManager.RoleExistsAsync(nameof(Role.Client)))
            await _roleManager.CreateAsync(new AppRole(nameof(Role.Client)));
    }
}
