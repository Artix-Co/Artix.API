namespace Artix.API.Core.DomainService.Services.XPRules;

using Contract.Features.Objects.Commands;
using Domain.Entities.User;
using Interfaces.TierCalculator;
using Interfaces.XPRules;
using Microsoft.AspNetCore.Identity;

internal sealed class XpRulesService : IXpRulesService
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITierCalculatorService _tierCalculatorService;

    public XpRulesService(IObjectCommandRepository objectCommandRepository, UserManager<AppUser> userManager,
        ITierCalculatorService tierCalculatorService)
    {
        _objectCommandRepository = objectCommandRepository;
        _userManager = userManager;
        _tierCalculatorService = tierCalculatorService;
    }

    /// <summary>
    /// محاسبه و اضافه کردن XP برای اسکن بار اول یک آبجکت
    /// فرض می‌شود UserObject از بیرون ایجاد شده است
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="objectId">شناسه آبجکت</param>
    /// <param name="seasonId">شناسه فصل (اختیاری)</param>
    /// <param name="cancellationToken"></param>
    public async Task CalculateXpForFirstScanAsync(long userId, Guid objectId, long? seasonId = null,
        CancellationToken cancellationToken = default)
    {
        // بررسی وجود کاربر
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found.");

        // دریافت اطلاعات آبجکت
        var objectEntity = await _objectCommandRepository.GetByIdAsync(objectId, cancellationToken);
        if (objectEntity == null)
            throw new Exception("Object not found.");

        // بررسی اینکه آبجکت قبلاً اسکن نشده باشد (مطابق سناریو)
        var userScan = user.UserScans.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == objectEntity.Id);
        if (userScan == null)
            throw new Exception("user scan not found.");

        // محاسبه Tier
        var (tierLevel, multiplier) = await _tierCalculatorService.CalculateTierAsync(userScan, cancellationToken);

        // محاسبه XP با اعمال multiplier
        long baseXp = objectEntity.IsSpecial ? 150 : 100;
        baseXp += objectEntity.Tier.GetValueOrDefault() * 10; // اضافه کردن XP بر اساس Tier آبجکت
        long xpToAdd = (long)(baseXp * multiplier); // اعمال multiplier از tier

        // به‌روزرسانی UserXp
        var userXp = user.UserXps.FirstOrDefault() ?? UserXp.Create(userId);
        if (!user.UserXps.Contains(userXp))
            user.AddUserXp(userXp);
        userXp.AddXp(xpToAdd);


        if (seasonId.HasValue)
        {
            var seasonProgress =
                user.UserSeasonProgresses.FirstOrDefault(sp => sp.UserId == userId && sp.SeasonId == seasonId.Value);
            if (seasonProgress == null)
            {
                seasonProgress = UserSeasonProgress.Create(userId, seasonId.Value, 0);
                user.AddUserSeasonProgress(seasonProgress); // اضافه کردن به لیست برای ردیابی
            }

            seasonProgress.AddXp((int)xpToAdd);
        }

        // ذخیره تغییرات با استفاده از Change Tracker
        await _userManager.UpdateAsync(user);
    }

    /// <summary>
    /// محاسبه و اضافه کردن XP برای اسکن بار چندم یک آبجکت
    /// فرض می‌شود UserObject از بیرون ایجاد شده است
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="objectId">شناسه آبجکت</param>
    /// <param name="seasonId">شناسه فصل (اختیاری)</param>
    /// <param name="isGoldenLevel">آیا شی به لِوِل طلایی رسیده؟ (برای آخرین کوییز)</param>
    /// <param name="cancellationToken"></param>
    public async Task CalculateXpForRepeatScanAsync(long userId, Guid objectId, long? seasonId = null,
        bool isGoldenLevel = false, CancellationToken cancellationToken = default)
    {
        // بررسی وجود کاربر
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found.");

        // دریافت اطلاعات آبجکت
        var objectEntity = await _objectCommandRepository.GetByIdAsync(objectId);
        if (objectEntity == null)
            throw new Exception("Object not found.");

        // بررسی اینکه آبجکت قبلاً اسکن شده باشد
        var userObject = user.UserScans.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == objectEntity.Id);
        if (userObject == null)
            throw new Exception("Object not previously scanned by user.");

        // ارتقا یا افزایش ScanCount
        if (!userObject.IsUpgraded)
            userObject.Upgrade();
        else if (isGoldenLevel)
            userObject.RecordScan(); // برای golden level

        /*

         * دلیل عدم استفاده از tierLevel در XpRulesService:

           تمرکز روی Multiplier در Gamification: طبق سناریوی Artix، multiplier از CalculateTierAsync برای تقویت XP (مثل boost در QR hunts، golden level، یا seasonal events) مهم‌تره، چون مستقیماً به پاداش‌ها (XP) و engagement وصل می‌شه. tierLevel بیشتر برای نمایش (مثل rankings یا کلکسیون) استفاده می‌شه و در محاسبه XP نقشی نداره.
           استفاده از Object.Tier: کد فعلی از objectEntity.Tier برای اضافه کردن XP پایه (مثل Tier * 10 یا Tier * 5) استفاده می‌کنه، که با منطق داخلی آبجکت هم‌خوانی داره و نیازی به tierLevel از UserScan نداره.
           جلوگیری از پیچیدگی: ترکیب tierLevel (از UserScan) با objectEntity.Tier می‌تونه لاجیک رو پیچیده کنه و با سناریو (که XP به ویژگی‌های آبجکت و multiplier وابسته‌ست) ناسازگار باشه.
           سناریوی Artix: tierLevel برای به‌روزرسانی‌های بعدی (مثل rankings یا ژورنال) ذخیره می‌شه، اما در محاسبه XP فعلی ضرورتی نداره، چون multiplier کافی پوشش می‌ده.

         */
        // محاسبه Tier
        var (tierLevel, multiplier) = await _tierCalculatorService.CalculateTierAsync(userObject, cancellationToken);

        // محاسبه XP با اعمال multiplier
        long baseXp = isGoldenLevel ? 200 : 50;
        baseXp += objectEntity.Tier.GetValueOrDefault() * 5; // XP کمتر برای اسکن‌های تکراری
        long xpToAdd = (long)(baseXp * multiplier); // اعمال multiplier از tier

        // به‌روزرسانی UserXp
        var userXp = user.UserXps.FirstOrDefault() ?? UserXp.Create(userId);
        if (!user.UserXps.Contains(userXp))
            user.AddUserXp(userXp);
        userXp.AddXp(xpToAdd);


        // اگر فصل فعال باشد، XP به UserSeasonProgress اضافه می‌شود
        if (seasonId.HasValue)
        {
            var seasonProgress =
                user.UserSeasonProgresses.FirstOrDefault(sp => sp.UserId == userId && sp.SeasonId == seasonId.Value);
            if (seasonProgress == null)
            {
                seasonProgress = UserSeasonProgress.Create(userId, seasonId.Value, 0);
                user.AddUserSeasonProgress(seasonProgress); // اضافه کردن به لیست برای ردیابی
            }

            seasonProgress.AddXp((int)xpToAdd);
        }

        // ذخیره تغییرات با استفاده از Change Tracker
        await _userManager.UpdateAsync(user);
    }
}
