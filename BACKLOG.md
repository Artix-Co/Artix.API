# Artix Admin Roles & Access Control Backlog

## هدف
پیاده‌سازی RBAC مدیریتی با مجوزهای مستقل و Scope داده، بدون اعطای دسترسی بیش از نیاز.

## تصمیم‌های لازم قبل از کدنویسی
- نقش‌های فاز اول: Super Admin، Museum Admin، Content Admin، Finance Admin، Technical Admin و Reviewer/Moderator.
- نقش‌های فاز بعد: Digital Asset Manager، Engagement & Gamification Manager، Customer Support Admin و Product/Business Analyst.
- آیا Reviewer/Moderator در فاز اول نقش مستقل باشد یا موقتاً با Content Admin مشترک بماند؟
- آیا Museum Admin فقط به موزه‌های تخصیص‌یافته دسترسی داشته باشد؟ (پیشنهاد: بله)
- آیا مجوزهای حساس مثل انتشار، حذف، بازپرداخت و تغییر امتیاز از مشاهده/ویرایش جدا باشند؟ (پیشنهاد: بله)

## کارهای فاز اول
- [x] تعریف منابع و عملیات مجوزها: View/Create/Edit/Approve/Publish/Delete/Manage.
- [x] افزودن مدل Permission و اتصال Role به Permission.
- [x] تعریف Scope: Global، AssignedMuseum، AggregateOnly و Self/SupportLimited در صورت نیاز.
- [x] Seed نقش‌ها و مجوزهای پیش‌فرض.
- [x] اعمال authorization سمت سرور روی endpointهای مدیریتی.
- [x] اعمال محدودیت Museum Admin در query/repository، نه فقط در UI.
- [x] ثبت audit برای login، تغییر نقش/مجوز، تأیید/رد/انتشار، حذف و عملیات مالی حساس. (رخدادهای مدیریتی پنل در جدول AdminAuditEntries ذخیره می‌شوند.)
- [x] افزودن تست‌های authorization برای دسترسی افقی و عمودی.
- [x] ساخت migration و اجرای build/test.

## فاز دوم
- [x] تفکیک نقش‌های دارایی دیجیتال و گیمیفیکیشن.
- [x] گردش‌کار کامل ارسال، بررسی، اصلاح، رد و انتشار محتوا. (چرخه وضعیت آثار و migration اضافه شد.)
- [x] مجوزهای تحلیلی فقط‌خواندنی و محدودسازی داده شخصی/مالی. (مجوز Analytics و role محدود شد؛ endpoint تحلیلی در backlog محصول جداست.)

## فاز سوم
- [ ] پشتیبانی کاربران و ticket scope.
- [ ] دسترسی قراردادی چندموزه‌ای.
- [ ] دسترسی اضطراری زمان‌دار و بازبینی دوره‌ای مجوزها.
- [ ] MFA برای نقش‌های حساس.
