namespace Artix.API.Infra.Identity.Services.TokenProvider;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Contract.Configs.Authentication;
using Core.Contract.Primitives.Infra.Identity;
using Core.Domain.Entities.User;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.AspNetCore.Http;
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
    private readonly ITokenRevocationStore _revocationStore;
    private readonly IUserSessionService _userSessionService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public JwtTokenGenerator(
        UserManager<AppUser> userManager,
        IOptions<AuthenticationSettings> authenticationSettings,
        ILogger<JwtTokenGenerator> logger, ITokenRevocationStore revocationStore,
        IUserSessionService userSessionService, IHttpContextAccessor httpContextAccessor)
    {
        this._userManager = userManager;
        this._logger = logger;
        this._revocationStore = revocationStore;
        this._userSessionService = userSessionService;
        this._httpContextAccessor = httpContextAccessor;
        this._tokenHandler = new JwtSecurityTokenHandler();
        this._signingKey = authenticationSettings.Value.IssuerSigningKey;
        this._issuer = authenticationSettings.Value.Issuer;
        this._audience = authenticationSettings.Value.Audience;
        this._accessTokenExpireTimeInSeconds = authenticationSettings.Value.AccessTokenExpireSeconds;
        this._refreshTokenExpireTimeInDays = authenticationSettings.Value.RefreshTokenExpireDays;
    }


    public async Task<JwtTokenResult> GenerateTokensAsync(
        AppUser user,
        bool forceRefreshToken = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "JWT generation started. UserId={UserId}, ForceRefresh={ForceRefresh}",
            user.Id,
            forceRefreshToken);

        // -------------------------
        // Resolve roles & claims
        // -------------------------
        _logger.LogDebug("Fetching roles and claims. UserId={UserId}", user.Id);

        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        _logger.LogDebug(
            "Roles and claims fetched. UserId={UserId}, RolesCount={RolesCount}, ClaimsCount={ClaimsCount}",
            user.Id,
            roles.Count,
            userClaims.Count);

        // -------------------------
        // Generate JTI
        // -------------------------
        var jti = Guid.CreateVersion7().ToString();

        _logger.LogDebug(
            "JTI generated. UserId={UserId}, Jti={Jti}",
            user.Id,
            jti);

        // -------------------------
        // Access token expiry
        // -------------------------
        var accessTokenExpiresAt =
            DateTime.UtcNow.AddSeconds(_accessTokenExpireTimeInSeconds);

        _logger.LogDebug(
            "Access token expiry calculated. UserId={UserId}, ExpiresAt={ExpiresAt}",
            user.Id,
            accessTokenExpiresAt);

        // -------------------------
        // Resolve new user state
        // -------------------------
        _logger.LogDebug(
            "Resolving new user state. UserId={UserId}",
            user.Id);

        var isNewUser = await IsNewUserAsync(user, cancellationToken);

        _logger.LogInformation(
            "User state resolved. UserId={UserId}, IsNewUser={IsNewUser}",
            user.Id,
            isNewUser);

        // -------------------------
        // Build JWT claims
        // -------------------------
        _logger.LogDebug(
            "Building JWT claims. UserId={UserId}",
            user.Id);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("isNewUser", isNewUser.ToString().ToLowerInvariant()),
            new("accessTokenExpireDateTime", accessTokenExpiresAt.ToString("O"))
        };

        authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        authClaims.AddRange(userClaims.Where(c => c.Type == "ClientType"));

        _logger.LogDebug(
            "JWT claims built. UserId={UserId}, TotalClaims={ClaimsCount}",
            user.Id,
            authClaims.Count);

        // -------------------------
        // Revoke previous access token if exists
        // -------------------------
        if (!isNewUser)
        {
            var existingAccessToken =
                await _userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "access_token");

            _logger.LogInformation(
                "Existing access token found. Evaluating revocation. UserId={UserId}",
                user.Id);

            var existingJwt = _tokenHandler.ReadJwtToken(existingAccessToken);
            var oldJti = existingJwt.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(oldJti) && oldJti != jti)
            {
                _logger.LogWarning(
                    "Revoking previous access token. UserId={UserId}, OldJti={OldJti}",
                    user.Id,
                    oldJti);

                await _revocationStore.RevokeAsync(oldJti, existingJwt.ValidTo);
            }
        }

        // -------------------------
        // Create access token
        // -------------------------
        _logger.LogInformation(
            "Creating access token. UserId={UserId}, Jti={Jti}",
            user.Id,
            jti);

        var accessToken = CreateJwtToken(authClaims, accessTokenExpiresAt);

        await StoreAccessTokenAsync(
            user,
            accessToken,
            accessTokenExpiresAt,
            cancellationToken);

        _logger.LogInformation(
            "Access token stored successfully. UserId={UserId}, ExpiresAt={ExpiresAt}",
            user.Id,
            accessTokenExpiresAt);

        // -------------------------
        // Refresh token handling
        // -------------------------
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

                _logger.LogInformation(
                    "Reusing existing refresh token. UserId={UserId}",
                    user.Id);
            }
            else
            {
                refreshToken = GenerateSecureRefreshToken();
                refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpireTimeInDays);

                _logger.LogInformation(
                    "Generating new refresh token. UserId={UserId}, ExpiresAt={ExpiresAt}",
                    user.Id,
                    refreshTokenExpiresAt);

                await StoreRefreshTokenAsync(
                    user,
                    refreshToken,
                    refreshTokenExpiresAt,
                    cancellationToken);
            }
        }
        else
        {
            refreshToken = GenerateSecureRefreshToken();
            refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpireTimeInDays);

            _logger.LogInformation(
                "Force refresh token requested. New refresh token generated. UserId={UserId}, ExpiresAt={ExpiresAt}",
                user.Id,
                refreshTokenExpiresAt);

            await StoreRefreshTokenAsync(
                user,
                refreshToken,
                refreshTokenExpiresAt,
                cancellationToken);
        }

        // -------------------------
        // Record user session
        // -------------------------
        _logger.LogInformation(
            "Recording user session. UserId={UserId}, Jti={Jti}",
            user.Id,
            jti);

        var hashedRefreshToken = Hash(refreshToken);
        await _userSessionService.RecordLoginAsync(
            userId: user.Id,
            jwtId: jti,
            refreshTokenHash: hashedRefreshToken,
            ipAddress: GetClientIp(),
            userAgent: GetUserAgent(),
            lifetime: TimeSpan.FromSeconds(_accessTokenExpireTimeInSeconds),
            cancellationToken);

        _logger.LogInformation(
            "User session recorded successfully. UserId={UserId}, Jti={Jti}",
            user.Id,
            jti);

        // -------------------------
        // Done
        // -------------------------
        _logger.LogInformation(
            "JWT generation completed successfully. UserId={UserId}, Jti={Jti}",
            user.Id,
            jti);

        return new JwtTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            Jti = jti,
            RefreshTokenHash = hashedRefreshToken
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

    private string GetClientIp()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return "unknown";

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.FirstOrDefault()?.Split(',').FirstOrDefault() ?? "unknown";

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string GetUserAgent()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.Request.Headers["User-Agent"].ToString() ?? "unknown";
    }

    private static string Hash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value to hash cannot be null or empty.", nameof(value));

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    private async Task<bool> IsNewUserAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        var existingAccessToken =
            await _userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "access_token");

        return string.IsNullOrEmpty(existingAccessToken);
    }
}
