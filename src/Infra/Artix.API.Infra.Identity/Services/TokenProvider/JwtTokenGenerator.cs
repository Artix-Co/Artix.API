namespace Artix.API.Infra.Identity.Services.TokenProvider;

using Core.Contract.Primitives.Infra.Identity;
using Core.Domain.Entities.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Contract.Configs.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<JwtTokenGenerator> _logger;
    private readonly AuthenticationSettings _settings;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IDeviceFingerprintService _deviceFingerprintService;
    private readonly IUserSessionService _userSessionService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public JwtTokenGenerator(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<AuthenticationSettings> settings,
        ILogger<JwtTokenGenerator> logger, IDeviceFingerprintService deviceFingerprintService,
        IUserSessionService userSessionService, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _deviceFingerprintService = deviceFingerprintService;
        _userSessionService = userSessionService;
        _httpContextAccessor = httpContextAccessor;
        _settings = settings.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<JwtTokenResult> GenerateTokensAsync(
        AppUser user,
        bool forceRefreshToken = false,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Token generation started. UserId={UserId}", user.Id);

        // ----------------------------
        // Load identity data
        // ----------------------------
        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var securityStamp = await _userManager.GetSecurityStampAsync(user);

        // ----------------------------
        // Core identifiers
        // ----------------------------
        var jti = Guid.CreateVersion7().ToString();
        var sessionId = Guid.CreateVersion7();
        var familyId = Guid.CreateVersion7();
        var now = DateTime.UtcNow;

        var accessTokenExpiresAt =
            now.AddSeconds(_settings.AccessTokenExpireSeconds);

        var refreshTokenExpiresAt =
            now.AddDays(_settings.RefreshTokenExpireDays);

        var refreshToken = GenerateSecureRefreshToken();
        var refreshTokenHash = Hash(refreshToken);
        var isNewUser = await IsNewUserAsync(user, cancellationToken);

        var context = _httpContextAccessor.HttpContext
                       ?? throw new UnauthorizedAccessException("An HTTP context is required to issue a token.");
        var fingerPrint = _deviceFingerprintService.Create(context);


        // ----------------------------
        // Build JWT claims
        // ----------------------------
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, jti),
            new("sid", sessionId.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("isNewUser", isNewUser.ToString().ToLowerInvariant()),
            new("accessTokenExpireDateTime", accessTokenExpiresAt.ToString("O")),
            new("sst", securityStamp),
            new("fingerPrint", fingerPrint),
        };

        authClaims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        foreach (var role in roles)
        {
            var roleEntity = await _roleManager.FindByNameAsync(role);
            if (roleEntity is null)
                continue;

            var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
            authClaims.AddRange(roleClaims.Where(x => x.Type == "permission"));
        }
        authClaims.AddRange(claims);

        // ----------------------------
        // Create access token
        // ----------------------------
        var accessToken = CreateJwt(authClaims, accessTokenExpiresAt);

        await _userSessionService.CreateAsync(
            user.Id, sessionId, familyId, jti, refreshTokenHash, refreshTokenExpiresAt,
            fingerPrint, fingerPrint,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            context.Request.Headers.UserAgent.ToString(), cancellationToken);

        _logger.LogInformation(
            "Token generated successfully. UserId={UserId}, Jti={Jti}",
            user.Id,
            jti);

        // ----------------------------
        // Return (NO persistence here)
        // ----------------------------
        return new JwtTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            Jti = jti,
            RefreshTokenHash = refreshTokenHash
        };
    }

    private string CreateJwt(IEnumerable<Claim> claims, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.IssuerSigningKey));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return _tokenHandler.WriteToken(token);
    }

    private static string GenerateSecureRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    private async Task<bool> IsNewUserAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        // بررسی کنید که آیا کاربر قبلاً لاگین کرده یا نه
        // روش بهتر: استفاده از LastLoginDate در جدول Users
        var lastSession = await _userSessionService.GetLastSessionByUserIdAsync(user.Id, cancellationToken);
        return lastSession == null;
    }
}
