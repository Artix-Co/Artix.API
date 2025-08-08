namespace Artix.API.Core.DomainService.Users;

using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contract.Configs.Authentication;
using Contract.Features.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Token;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly ILogger<JwtTokenGenerator> _logger;
    private readonly string _signingKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpireTimeInSeconds;
    private readonly int _refreshTokenExpireTimeInDays;

    public JwtTokenGenerator(
        UserManager<AppUser> userManager,
        IOptions<AuthenticationSettings> authenticationSettings,
        ILogger<JwtTokenGenerator> logger)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();

        _signingKey = authenticationSettings.Value.IssuerSigningKey;
        _issuer = authenticationSettings.Value.Issuer;
        _audience = authenticationSettings.Value.Audience;
        _accessTokenExpireTimeInSeconds = authenticationSettings.Value.AccessTokenExpireSeconds;
        _refreshTokenExpireTimeInDays = authenticationSettings.Value.RefreshTokenExpireDays;
    }

    public async Task<JwtTokenResult> GenerateTokensAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating tokens for user {UserId} - {Username}", user.Id, user.UserName);

        var roles = await _userManager.GetRolesAsync(user);
        _logger.LogDebug("Fetched {RoleCount} roles for user {UserId}", roles.Count, user.Id);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessTokenExpiresAt = DateTime.UtcNow.AddSeconds(_accessTokenExpireTimeInSeconds);
        var accessToken = CreateJwtToken(authClaims, accessTokenExpiresAt);

        _logger.LogDebug("Access token generated for user {UserId} with expiry {Expiry}", user.Id,
            accessTokenExpiresAt);

        var refreshToken = GenerateSecureRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpireTimeInDays);

        _logger.LogDebug("Refresh token generated for user {UserId} with expiry {Expiry}", user.Id,
            refreshTokenExpiresAt);

        await StoreRefreshTokenAsync(user, refreshToken, refreshTokenExpiresAt, cancellationToken);

        return new JwtTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }

    private string CreateJwtToken(IEnumerable<Claim> claims, DateTime expiresAt)
    {
        var key = Encoding.UTF8.GetBytes(_signingKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _issuer,
            Audience = _audience
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    private static string GenerateSecureRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task StoreRefreshTokenAsync(AppUser user, string refreshToken, DateTime expiresAt,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Storing refresh token for user {UserId}", user.Id);

        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");
        var result = await _userManager.SetAuthenticationTokenAsync(user, "ArtixApp", "refresh_token", refreshToken);

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to store refresh token for user {UserId}: {Errors}", user.Id,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new InvalidOperationException("Failed to store refresh token.");
        }

        _logger.LogInformation("Refresh token successfully stored for user {UserId}", user.Id);
    }
}
