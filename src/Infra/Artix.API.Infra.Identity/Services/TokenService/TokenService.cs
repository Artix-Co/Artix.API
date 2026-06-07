namespace Artix.API.Infra.Identity.Services.TokenService;

using Core.Contract.Configs.Authentication;
using Core.Contract.Primitives.Infra.Identity;
using Core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly int _refreshTokenExpireDays;

    public TokenService(
        UserManager<AppUser> userManager, 
        IJwtTokenGenerator jwtTokenGenerator,
        IOptions<AuthenticationSettings> options)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenExpireDays = options.Value.RefreshTokenExpireDays;
    }

    public async Task<JwtTokenResult> ReNewAccessTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // 1. پیدا کردن کاربر بر اساس RefreshToken
        var user = await _userManager.Users
            .Include(u => u.Tokens)
            .FirstOrDefaultAsync(u =>
                u.Tokens.Any(t => t.LoginProvider == "ArtixApp" &&
                                  t.Name == "refresh_token" &&
                                  t.Value == refreshToken), cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        // 2. پیدا کردن رکورد RefreshToken
        var refreshTokenRecord = user.Tokens
            .FirstOrDefault(t => t.LoginProvider == "ArtixApp" &&
                                 t.Name == "refresh_token" &&
                                 t.Value == refreshToken);

        if (refreshTokenRecord is null)
        {
            throw new UnauthorizedAccessException("Refresh token not found.");
        }

        // 3. بررسی انقضای RefreshToken (نیاز به فیلد ExpiresAt در IdentityUserToken)
        // اگر چنین فیلدی ندارید، باید تاریخ انقضا را در جای دیگری ذخیره کنید
        // راهکار موقت: از فیلد Value استفاده کنید و تاریخ را در آن ذخیره کنید (غیراستاندارد)
        // راهکار بهتر: مهاجرت دیتابیس و اضافه کردن ستون ExpiresAt به جدول AspNetUserTokens
        
        if (IsRefreshTokenExpired(refreshTokenRecord))
        {
            // توکن منقضی شده - آن را پاک کن
            await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");
            throw new UnauthorizedAccessException("Refresh token has expired. Please login again.");
        }

        // 4. REFRESH TOKEN ROTATION - همیشه توکن جدید بده و قدیمی را باطل کن
        // این کار امنیت را بالا می‌برد (اگر توکن به سرقت رفته باشد، بلافاصله باطل می‌شود)
        
        // باطل کردن RefreshToken قدیمی
        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "refresh_token");
        
        // 5. اجرای Revoke همه سشن‌ها با JWT قدیمی (اختیاری - بستگی به نیاز امنیتی دارد)
        // برای امنیت بالاتر: وقتی RefreshToken استفاده شد، همه توکن‌های قبلی را باطل کن
        
        // 6. تولید توکن‌های جدید (forceRefreshToken = true تا حتماً RefreshToken جدید بسازد)
        var result = await _jwtTokenGenerator.GenerateTokensAsync(
            user, 
            forceRefreshToken: true,  // حتماً RefreshToken جدید بساز
            cancellationToken);

        return result;
    }

    private bool IsRefreshTokenExpired(IdentityUserToken<long> tokenRecord)
    {
        // روش 1: اگر فیلد ExpiresAt دارید (توصیه می‌شود)
        // return tokenRecord.ExpiresAt < DateTime.UtcNow;
        
        // روش 2: روش موقت - تاریخ را در Value با فرمت خاص ذخیره کنید
        // مثال: "Base64Token|ExpiryDate"
        if (tokenRecord.Value.Contains('|'))
        {
            var parts = tokenRecord.Value.Split('|');
            if (parts.Length == 2 && DateTime.TryParse(parts[1], out var expiry))
            {
                return expiry < DateTime.UtcNow;
            }
        }
        
        // روش 3: اگر هیچکدام را ندارید، از مقدار تنظیمات استفاده کنید
        // (این روش ایده‌آل نیست چون تاریخ واقعی انقضا را نمی‌داند)
        var fallbackExpiry = GetTokenCreationDate(tokenRecord)?.AddDays(_refreshTokenExpireDays);
        return fallbackExpiry.HasValue && fallbackExpiry.Value < DateTime.UtcNow;
    }
    
    private DateTime? GetTokenCreationDate(IdentityUserToken<long> tokenRecord)
    {
        // این یک روش تخمینی است - بهتر است حتماً فیلد CreatedAt به دیتابیس اضافه کنید
        // می‌توانید از ترکیب TokenValue + یک metadata استفاده کنید
        return null;
    }
}
