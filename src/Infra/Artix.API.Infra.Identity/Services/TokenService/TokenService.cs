namespace Artix.API.Infra.Identity.Services.TokenService;

using System.Security.Cryptography;
using System.Text;
using Core.Contract.Primitives.Infra.Identity;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;

public sealed class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserSessionService _userSessionService;
    private readonly IDistributedLockService _lockService;

    public TokenService(
        UserManager<AppUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserSessionService userSessionService,
        IDistributedLockService lockService)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userSessionService = userSessionService;
        _lockService = lockService;
    }

    public async Task<JwtTokenResult> ReNewAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var refreshTokenHash = Hash(refreshToken);
        await using var refreshLock = await _lockService.TryAcquireAsync(
            $"refresh-token:{refreshTokenHash}", TimeSpan.FromSeconds(30), cancellationToken);

        if (refreshLock is null || !refreshLock.Acquired)
            throw new UnauthorizedAccessException("Refresh token is already being used.");

        var session = await _userSessionService.FindByRefreshTokenHashAsync(
            refreshTokenHash, cancellationToken);

        if (session is null || !session.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await _userManager.FindByIdAsync(session.UserId.ToString());
        if (user is null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        await _userSessionService.RevokeAsync(session.SessionId, "Refresh token rotated", cancellationToken);

        return await _jwtTokenGenerator.GenerateTokensAsync(
            user, forceRefreshToken: true, cancellationToken);
    }

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
