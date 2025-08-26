namespace Artix.API.Core.DomainService.Services.XPRules;

using Contract.Features.Objects.Commands;
using Domain.Entities.User;
using Interfaces.XPRules;
using Microsoft.AspNetCore.Identity;

internal sealed class XpRulesService : IXpRulesService
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly UserManager<AppUser> _userManager;

    public XpRulesService(IObjectCommandRepository objectCommandRepository, UserManager<AppUser> userManager)
    {
        _objectCommandRepository = objectCommandRepository;
        _userManager = userManager;
    }

    /// <summary>
    /// محاسبه و اضافه کردن XP برای اسکن بار اول یک آبجکت
    /// فرض می‌شود UserObject از بیرون ایجاد شده است
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="objectId">شناسه آبجکت</param>
    /// <param name="seasonId">شناسه فصل (اختیاری)</param>
    public async Task CalculateXpForFirstScanAsync(long userId, Guid objectId, long? seasonId = null)
    {
        // بررسی وجود کاربر
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found.");

        // دریافت اطلاعات آبجکت
        var objectEntity = await _objectCommandRepository.GetByIdAsync(objectId);
        if (objectEntity == null)
            throw new Exception("Object not found.");

        // بررسی اینکه آبجکت قبلاً اسکن نشده باشد (مطابق سناریو)
        var userObject = user.UserObjects.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == objectEntity.Id);
        if (userObject != null)
            throw new Exception("Object already scanned by user.");

        // محاسبه XP بر اساس ویژگی‌های آبجکت
        long xpToAdd = objectEntity.IsSpecial ? 150 : 100;
        if (objectEntity.Tier.HasValue)
            xpToAdd += objectEntity.Tier.Value * 10; // اضافه کردن XP بر اساس Tier

        // به‌روزرسانی UserXp (امتیازات کلی)
        var userXp = user.UserXps.FirstOrDefault();
        if (userXp == null)
        {
            userXp = UserXp.Create(userId);
            user.AddUserXp(userXp); // اضافه کردن به لیست برای ردیابی توسط EF Core
        }

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

    /// <summary>
    /// محاسبه و اضافه کردن XP برای اسکن بار چندم یک آبجکت
    /// فرض می‌شود UserObject از بیرون ایجاد شده است
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="objectId">شناسه آبجکت</param>
    /// <param name="seasonId">شناسه فصل (اختیاری)</param>
    /// <param name="isGoldenLevel">آیا شی به لِوِل طلایی رسیده؟ (برای آخرین کوییز)</param>
    public async Task CalculateXpForRepeatScanAsync(long userId, Guid objectId, long? seasonId = null,
        bool isGoldenLevel = false)
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
        var userObject = user.UserObjects.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == objectEntity.Id);
        if (userObject == null)
            throw new Exception("Object not previously scanned by user.");

        // محاسبه XP برای اسکن مجدد
        long xpToAdd = isGoldenLevel ? 200 : 50; // 200 برای لِوِل طلایی، 50 برای اسکن معمولی
        if (objectEntity.Tier.HasValue)
            xpToAdd += objectEntity.Tier.Value * 5; // XP کمتر برای اسکن‌های بعدی

        // به‌روزرسانی UserXp (امتیازات کلی)
        var userXp = user.UserXps.FirstOrDefault();
        if (userXp == null)
        {
            userXp = UserXp.Create(userId);
            user.AddUserXp(userXp); // اضافه کردن به لیست برای ردیابی توسط EF Core
        }

        userXp.AddXp(xpToAdd);

        // ارتقا UserObject
        if (!userObject.IsUpgraded)
        {
            userObject.Upgrade();
        }
        else if (isGoldenLevel)
        {
            userObject.RecordScan(); // افزایش ScanCount برای لِوِل طلایی
        }

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
