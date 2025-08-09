namespace Artix.API.Infra.Identity.Services.TokenService;

using Core.Contract.Features.Tokens;
using Core.Domain.Entities.User;
using Artix.API.Infra.Identity.Interfaces.TokenProvider;
using Artix.API.Infra.Identity.Interfaces.TokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public sealed class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public TokenService(UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        this._userManager = userManager;
        this._jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<JwtTokenResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await this._userManager.Users
            .FirstOrDefaultAsync(u =>
                u.Tokens.Any(t => t.LoginProvider == "ArtixApp" &&
                                  t.Name == "refresh_token" &&
                                  t.Value == refreshToken), cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        return await this._jwtTokenGenerator.GenerateTokensAsync(user, cancellationToken);
    }
}
