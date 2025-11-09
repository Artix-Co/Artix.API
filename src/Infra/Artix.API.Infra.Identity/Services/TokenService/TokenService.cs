namespace Artix.API.Infra.Identity.Services.TokenService;

using Core.Contract.Features.Tokens;
using Core.Domain.Entities.User;
using Core.Contract.Configs.Authentication;
using Core.Contract.Primitives.Infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly int _expiresDays;

    public TokenService(UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator,IOptions<AuthenticationSettings> _options)
    {
        this._userManager = userManager;
        this._jwtTokenGenerator = jwtTokenGenerator;
        this._expiresDays = _options.Value.RefreshTokenExpireDays;
    }

    public async Task<JwtTokenResult> ReNewAccessTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var user = await this._userManager.Users
            .Include(u => u.Tokens)
            .FirstOrDefaultAsync(u =>
                u.Tokens.Any(t => t.LoginProvider == "ArtixApp" &&
                                  t.Name == "refresh_token" &&
                                  t.Value == refreshToken), cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        // چک کردن انقضای رفرش توکن
        bool isRefreshTokenExpired = user.Tokens
            .Any(t => t.LoginProvider == "ArtixApp" &&
                      t.Name == "refresh_token" &&
                      t.Value == refreshToken &&
                      DateTime.UtcNow >= DateTime.UtcNow.AddDays(this._expiresDays)); // فرض می‌کنیم 30 روز انقضا

        return await this._jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: isRefreshTokenExpired,
            cancellationToken);
    }
}
