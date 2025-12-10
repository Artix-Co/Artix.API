namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetLogout;

using System.IdentityModel.Tokens.Jwt;
using Primitives;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Users.Client.Queries.GetLogout;
using Domain.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler
internal sealed class GetLogoutQueryHandler : QueryHandlerBase<GetLogoutQuery, LogoutDto>
{
    private readonly ITokenRevocationStore _revocationStore;


    public GetLogoutQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        ITokenRevocationStore revocationStore) : base(httpContextAccessor, userManager)
    {
        this._revocationStore = revocationStore;
    }

    public override async Task<Result<LogoutDto>> Handle(GetLogoutQuery query, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var accessToken = await this._userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiry =
                DateTimeOffset.FromUnixTimeSeconds(jwt.ValidTo.ToUniversalTime().Ticks / TimeSpan.TicksPerSecond);

            if (jti != null)
                await this._revocationStore.RevokeAsync(jti, expiry);
        }

        await this._userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");
        await this._userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");

        if (this._httpContextAccessor.HttpContext != null)
            await this._httpContextAccessor.HttpContext.SignOutAsync();

        return Result<LogoutDto>.Success(new LogoutDto());
    }
}
