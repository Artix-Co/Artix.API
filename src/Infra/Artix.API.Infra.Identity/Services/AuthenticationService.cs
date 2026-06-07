namespace Artix.API.Infra.Identity.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Core.Contract.Primitives.Infra.Identity;
using Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Login;
using Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Logout;
using Core.Contract.Primitives.Infra.Identity.Authentication.Client.Logout;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.Entities.User;
using Core.Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using IAuthenticationService = Core.Contract.Primitives.Infra.Identity.Authentication.IAuthenticationService;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserSessionService _userSessionService;
    private readonly ITokenRevocationStore _tokenRevocationStore;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserSessionService userSessionService,
        ITokenRevocationStore tokenRevocationStore,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userSessionService = userSessionService;
        _tokenRevocationStore = tokenRevocationStore;
        _httpContextAccessor = httpContextAccessor;
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
