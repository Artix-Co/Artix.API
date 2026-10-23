namespace Artix.API.Infra.Identity.Services;

using System.Security.Claims;
using Core.Contract.Primitives.Infra.Identity;
using Core.Contract.Primitives.Infra.Identity.Authentication;
using Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Login;
using Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Logout;
using Core.Contract.Primitives.Infra.Identity.Authentication.Client.Logout;
using Core.Domain.Entities.User;
using Core.Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserSessionService _userSessionService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator,
        IUserSessionService userSessionService, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userSessionService = userSessionService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ClientLogoutResponse> ClientLogoutAsync(ClientLogoutRequest request, CancellationToken cancellationToken = default)
    {
        await LogoutInternalAsync(cancellationToken);
        return new ClientLogoutResponse();
    }

    public async Task<AdminLogoutResponse> AdminLogoutAsync(AdminLogoutRequest request, CancellationToken cancellationToken = default)
    {
        await LogoutInternalAsync(cancellationToken);
        return new AdminLogoutResponse();
    }

    public async Task<AdminLoginResponse> AdminLoginAsync(AdminLoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException();

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(nameof(Role.Admin)))
            throw new UnauthorizedAccessException();

        var tokens = await _jwtTokenGenerator.GenerateTokensAsync(user, true, cancellationToken);
        return new AdminLoginResponse(tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt, user.UserName!, user.DisplayName, roles.ToArray().AsReadOnly());
    }

    private async Task LogoutInternalAsync(CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userId, out var id))
            throw new UnauthorizedAccessException();

        await _userSessionService.RevokeAllAsync(id, "logout", cancellationToken);
    }
}
