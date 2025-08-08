namespace Artix.API.Core.DomainService.Users.Token;

using Contract.Features.Tokens;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public sealed class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public TokenService(UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<JwtTokenResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u =>
                u.Tokens.Any(t => t.LoginProvider == "ArtixApp" &&
                                  t.Name == "refresh_token" &&
                                  t.Value == refreshToken), cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        return await _jwtTokenGenerator.GenerateTokensAsync(user, cancellationToken);
    }
}
