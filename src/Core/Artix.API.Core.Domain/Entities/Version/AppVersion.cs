namespace Artix.API.Core.Domain.Entities.Version;

using Common;

using System;
using System.ComponentModel.DataAnnotations;

public class AppVersion : AggregateRoot
{
    // فیلدهای اصلی برای نسخه‌گذاری (Semantic Versioning)
    public int Major { get; private set; }
    public int Minor { get; private set; }
    public int Patch { get; private set; }
    
    // رشته نسخه به‌صورت محاسبه‌شده (برای نمایش یا کوئری)
    public string VersionString { get; private set; }

    // آیا این نسخه اجباری است؟
    public bool IsRequired { get; private set; }

    // آیا نسخه‌های پایین‌تر از این پشتیبانی می‌شوند؟
    public bool MinSupported { get; private set; }

    // توضیحات تغییرات نسخه (اختیاری)
    [MaxLength(500)]
    public string? Description { get; private set; }
    
    
    // سازنده پیش‌فرض برای EF Core
    private AppVersion() { }

    // سازنده برای اطمینان از مقادیر معتبر
    private AppVersion(int major, int minor, int patch, bool isRequired, bool minSupported, string? description = null)
    {
        if (major < 0 || minor < 0 || patch < 0)
            throw new ArgumentException("Version numbers cannot be negative.");

        Major = major;
        Minor = minor;
        Patch = patch;
        IsRequired = isRequired;
        MinSupported = minSupported;
        Description = description?.Length > 500 ? description.Substring(0, 500) : description;
    }

    public static AppVersion Create(int major, int minor, int patch, bool isRequired, bool minSupported, string? description = null)
    {
        return new AppVersion(major, minor, patch, isRequired, minSupported, description);
    }
    
    


    // متد برای مقایسه نسخه‌ها
    public bool IsNewerThan(AppVersion other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        return (Major > other.Major) ||
               (Major == other.Major && Minor > other.Minor) ||
               (Major == other.Major && Minor == other.Minor && Patch > other.Patch);
    }

    // متد برای چک کردن نیاز به آپدیت
    public bool RequiresUpdate(AppVersion clientVersion)
    {
        if (clientVersion == null) throw new ArgumentNullException(nameof(clientVersion));

        // اگر نسخه فعلی اجباری است و نسخه کلاینت قدیمی‌تر است
        if (IsRequired && IsNewerThan(clientVersion))
            return true;

        // اگر نسخه کلاینت پایین‌تر از حداقل نسخه پشتیبانی‌شده است
        if (!MinSupported && IsNewerThan(clientVersion))
            return true;

        return false;
    }

    // متد برای به‌روزرسانی اطلاعات نسخه
    public void Update(int major, int minor, int patch, bool isRequired, bool minSupported, string? description = null)
    {
        if (major < 0 || minor < 0 || patch < 0)
            throw new ArgumentException("Version numbers cannot be negative.");

        Major = major;
        Minor = minor;
        Patch = patch;
        IsRequired = isRequired;
        MinSupported = minSupported;
        Description = description?.Length > 500 ? description.Substring(0, 500) : description;
        ModifiedAt = DateTime.UtcNow;
    }

    // متد برای غیرفعال کردن نسخه
    public void Deactivate()
    {
        IsDeleted = true;
        ModifiedAt = DateTime.UtcNow;
    }
}
