namespace Artix.API.Core.ApplicationService.Features.Users.Queries.Logout;

using System.IdentityModel.Tokens.Jwt;
using Contract.Features.Users.Queries.Logout;
using Contract.Primitives.Infra.Redis;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validator for this handler
internal sealed class LogoutQueryHandler : QueryHandlerBase<GetLogoutQuery, LogoutDto>
{
    private readonly ITokenRevocationStore _revocationStore;


    public LogoutQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        ITokenRevocationStore revocationStore) : base(httpContextAccessor, userManager)
    {
        this._revocationStore = revocationStore;
    }

    public override async Task<Result<LogoutDto>> Handle(GetLogoutQuery query, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var accessToken = await _userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiry =
                DateTimeOffset.FromUnixTimeSeconds(jwt.ValidTo.ToUniversalTime().Ticks / TimeSpan.TicksPerSecond);

            if (jti != null)
                await _revocationStore.RevokeAsync(jti, expiry);
        }

        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");
        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");

        if (_httpContextAccessor.HttpContext != null)
            await _httpContextAccessor.HttpContext.SignOutAsync();

        return Result<LogoutDto>.Success(new LogoutDto());
    }
}
