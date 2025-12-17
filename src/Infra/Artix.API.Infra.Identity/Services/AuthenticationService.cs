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
using Core.Domain.Entities.OTP.Enums;
using Core.Domain.Entities.User;
using Core.Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IAuthenticationService = Core.Contract.Primitives.Infra.Identity.Authentication.IAuthenticationService;

 
internal sealed class AuthenticationService : IAuthenticationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

    public async Task<ClientLoginResponse> ClientOtpLoginAsync(
        ClientLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = $"otp:{request.PhoneNumber}";

        var json = await _sessionStore.GetSessionAsync(key, cancellationToken);
        if (json is null)
            throw new UnauthorizedAccessException("OTP expired or not found.");

        var data = JsonSerializer.Deserialize<OtpSessionData>(json, JsonOptions)!;

        if (data.Attempts >= 3)
            throw new UnauthorizedAccessException("Too many failed attempts.");

        if (data.Code != request.OtpCode)
        {
            var updated = data with { Attempts = data.Attempts + 1 };
            await _sessionStore.SetSessionAsync(
                key,
                JsonSerializer.Serialize(updated, JsonOptions),
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
                .FirstOrDefaultAsync(
                    u => u.PhoneNumber == request.PhoneNumber,
                    cancellationToken);
        }

        if (user is null)
            throw new InvalidOperationException("Invalid OTP purpose or user state.");

        var tokens = await _jwtTokenGenerator.GenerateTokensAsync(
            user,
            true,
            cancellationToken);

        return new ClientLoginResponse(
            user.BusinessId,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt);
    }

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

    private async Task LogoutInternalAsync(CancellationToken ct)
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException();

        var userId = context.User
            .FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();

        await _userSessionService.RevokeAllAsync(long.Parse(userId), ct);

        var jti = context.User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var exp = context.User.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (jti is not null && long.TryParse(exp, out var unix))
        {
            await _tokenRevocationStore.RevokeAsync(
                jti,
                DateTimeOffset.FromUnixTimeSeconds(unix));
        }
    }

    public async Task<AdminLoginResponse> AdminLoginAsync(
        AdminLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is null)
            throw new UnauthorizedAccessException();

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException();

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(nameof(Role.Admin)))
            throw new UnauthorizedAccessException();

        var tokens = await _jwtTokenGenerator.GenerateTokensAsync(
            user,
            true,
            cancellationToken);

        return new AdminLoginResponse(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt,
            user.UserName!,
            user.DisplayName,
            roles.ToArray().AsReadOnly());
    }

    private async Task<AppUser> CreateClientUserAsync(string phoneNumber)
    {
        if (!await _roleManager.RoleExistsAsync(nameof(Role.Client)))
            await _roleManager.CreateAsync(new AppRole(nameof(Role.Client)));

        var user = new AppUser
        {
            UserName = $"user_{phoneNumber}",
            Email = $"{phoneNumber}@example.com",
            PhoneNumber = phoneNumber,
            DisplayName = phoneNumber
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new ApplicationException();

        await _userManager.AddToRoleAsync(user, nameof(Role.Client));
        return user;
    }
}
