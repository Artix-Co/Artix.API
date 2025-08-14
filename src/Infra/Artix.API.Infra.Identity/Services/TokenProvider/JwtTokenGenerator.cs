namespace Artix.API.Infra.Identity.Services.TokenProvider;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Contract.Configs.Authentication;
using Core.Contract.Features.Tokens;
using Core.Domain.Entities.User;
using Artix.API.Infra.Identity.Interfaces.TokenProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
        this._userManager = userManager;
        this._logger = logger;
        this._tokenHandler = new JwtSecurityTokenHandler();

        this._signingKey = authenticationSettings.Value.IssuerSigningKey;
        this._issuer = authenticationSettings.Value.Issuer;
        this._audience = authenticationSettings.Value.Audience;
        this._accessTokenExpireTimeInSeconds = authenticationSettings.Value.AccessTokenExpireSeconds;
        this._refreshTokenExpireTimeInDays = authenticationSettings.Value.RefreshTokenExpireDays;
    }


    public async Task<JwtTokenResult> GenerateTokensAsync(AppUser user, bool forceRefreshToken = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating tokens for user {UserId} - {Username}", user.Id, user.UserName);

        // Fetch roles and claims
        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user); // Fetch custom claims like ClientType

        // Build JWT claims
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        // Add roles to claims
        authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add custom claims (e.g., ClientType)
        authClaims.AddRange(userClaims.Where(c => c.Type == "ClientType")); // Only add ClientType claims

        // Generate access token
        var accessTokenExpiresAt = DateTime.UtcNow.AddSeconds(_accessTokenExpireTimeInSeconds);
        var accessToken = CreateJwtToken(authClaims, accessTokenExpiresAt);

        _logger.LogDebug("Access token generated for user {UserId} with expiry {Expiry}", user.Id,
            accessTokenExpiresAt);

        // Handle refresh token
        string refreshToken;
        DateTime refreshTokenExpiresAt;

        if (!forceRefreshToken)
        {
            var existingRefreshToken =
                await _userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");
            if (!string.IsNullOrEmpty(existingRefreshToken))
            {
                refreshToken = existingRefreshToken;
                refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpireTimeInDays);
                _logger.LogDebug("Using existing refresh token for user {UserId}, expiry: {Expiry}", user.Id,
                    refreshTokenExpiresAt);
            }
            else
            {
                refreshToken = GenerateSecureRefreshToken();
                refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpireTimeInDays);
                await StoreRefreshTokenAsync(user, refreshToken, refreshTokenExpiresAt, cancellationToken);
                _logger.LogDebug("New refresh token generated for user {UserId} with expiry {Expiry}", user.Id,
                    refreshTokenExpiresAt);
            }
        }
        else
        {
            refreshToken = GenerateSecureRefreshToken();
            refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpireTimeInDays);
            await StoreRefreshTokenAsync(user, refreshToken, refreshTokenExpiresAt, cancellationToken);
            _logger.LogDebug("Forced new refresh token generated for user {UserId} with expiry {Expiry}", user.Id,
                refreshTokenExpiresAt);
        }

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
        var key = Encoding.UTF8.GetBytes(this._signingKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = this._issuer,
            Audience = this._audience
        };

        var token = this._tokenHandler.CreateToken(tokenDescriptor);
        return this._tokenHandler.WriteToken(token);
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
        this._logger.LogInformation("Storing refresh token for user {UserId}", user.Id);

        await this._userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");
        var result =
            await this._userManager.SetAuthenticationTokenAsync(user, "ArtixApp", "refresh_token", refreshToken);

        if (!result.Succeeded)
        {
            this._logger.LogError("Failed to store refresh token for user {UserId}: {Errors}", user.Id,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new InvalidOperationException("Failed to store refresh token.");
        }

        this._logger.LogInformation("Refresh token successfully stored for user {UserId}", user.Id);
    }


    private async Task StoreAccessTokenAsync(AppUser user, string accessToken, DateTime expiresAt,
        CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Storing access token for user {UserId}", user.Id);


        await this._userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");
        var result = await this._userManager.SetAuthenticationTokenAsync(user, "ArtixApp", "access_token", accessToken);

        if (!result.Succeeded)
        {
            this._logger.LogError("Failed to store access token for user {UserId}: {Errors}", user.Id,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new InvalidOperationException("Failed to store access token.");
        }

        this._logger.LogInformation("Access token successfully stored for user {UserId}", user.Id);
    }
}
