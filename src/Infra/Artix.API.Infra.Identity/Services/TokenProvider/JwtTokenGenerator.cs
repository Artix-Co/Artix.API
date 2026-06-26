namespace Artix.API.Infra.Identity.Services.TokenProvider;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        // -------------------------
        // Generate JTI
        // -------------------------
        var jti = Guid.CreateVersion7().ToString();

        // -------------------------
        // Access token expiry
        // -------------------------
        var accessTokenExpiresAt = DateTime.UtcNow.AddSeconds(_accessTokenExpireTimeInSeconds);

        // -------------------------
        // بررسی NEW USER بر اساس session قبلی (نه access token در دیتابیس)
        // -------------------------
        var isNewUser = await IsNewUserAsync(user, cancellationToken);

        _logger.LogInformation(
            "User state resolved. UserId={UserId}, IsNewUser={IsNewUser}",
            user.Id,
            isNewUser);

        // -------------------------
        // Build JWT claims
        // -------------------------
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

        var fingerprint = CreateFingerprint();
        authClaims.Add(new Claim("fingerprint", fingerprint));

        // ==========================================
        // ❌ حذف شد: بخش Revoke قبلی که از دیتابیس می‌خواند
        // ==========================================
        // دیگر نیازی به revoke کردن access token قبلی نیست چون:
        // 1. AccessToken در دیتابیس ذخیره نمی‌شود
        // 2. Revoke از طریق Session و Redis انجام می‌شود
        // 3. وقتی کاربر لاگین می‌کند، session قبلی در LogoutInternalAsync باطل می‌شود

        // -------------------------
        // Create access token
        // -------------------------
        var accessToken = CreateJwtToken(authClaims, accessTokenExpiresAt);

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

                await StoreRefreshTokenAsync(user, refreshToken, refreshTokenExpiresAt, cancellationToken);
            }
        }
        else
        {
            refreshToken = GenerateSecureRefreshToken();
            refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpireTimeInDays);

            await StoreRefreshTokenAsync(user, refreshToken, refreshTokenExpiresAt, cancellationToken);
        }

        // -------------------------
        // Record user session
        // -------------------------
        var hashedRefreshToken = Hash(refreshToken);

        // قبل از ذخیره session جدید، sessionهای قبلی را باطل کن (اگر isNewUser نباشد)
        if (!isNewUser)
        {
            _logger.LogInformation(
                "Revoking all previous sessions for existing user. UserId={UserId}",
                user.Id);

            await _userSessionService.RevokeAllAsync(user.Id, cancellationToken);
        }

        await _userSessionService.RecordLoginAsync(
            userId: user.Id,
            jwtId: jti,
            refreshTokenHash: hashedRefreshToken,
            ipAddress: GetClientIp(),
            userAgent: GetUserAgent(),
            lifetime: TimeSpan.FromSeconds(_accessTokenExpireTimeInSeconds),
            cancellationToken);

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

    private string CreateFingerprint()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return "unknown";

        var fingerprint = new
        {
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            AcceptLanguage = context.Request.Headers["Accept-Language"].ToString(),
            SecChUa = context.Request.Headers["Sec-CH-UA"].ToString(),
            IpHash = Hash(GetClientIp())
        };

        var json = JsonSerializer.Serialize(fingerprint);
        return Hash(json);
    }

    private async Task StoreRefreshTokenAsync(
        AppUser user,
        string refreshToken,
        DateTime expiresAt,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Storing refresh token for user {UserId}", user.Id);

        // حذف توکن قبلی
        var removeResult = await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");

        if (!removeResult.Succeeded)
        {
            _logger.LogWarning("Failed to remove old refresh token for user {UserId}: {Errors}",
                user.Id, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
            // ادامه بده - شاید توکنی وجود نداشته باشد
        }

        // ذخیره توکن جدید
        var setResult = await _userManager.SetAuthenticationTokenAsync(user, "ArtixApp", "refresh_token", refreshToken);

        if (!setResult.Succeeded)
        {
            _logger.LogError("Failed to store refresh token for user {UserId}: {Errors}",
                user.Id, string.Join(", ", setResult.Errors.Select(e => e.Description)));
            throw new InvalidOperationException("Failed to store refresh token. Please try again.");
        }

        // اگر از مدل سفارشی با ExpiresAt استفاده می‌کنید
        // await SetRefreshTokenExpiryAsync(user, expiresAt, cancellationToken);

        _logger.LogInformation("Refresh token successfully stored for user {UserId}, expires at {ExpiresAt}",
            user.Id, expiresAt);
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
        // بررسی کنید که آیا کاربر قبلاً لاگین کرده یا نه
        // روش بهتر: استفاده از LastLoginDate در جدول Users
        var lastSession = await _userSessionService.GetLastSessionByUserIdAsync(user.Id, cancellationToken);
        return lastSession == null;
    }
}
