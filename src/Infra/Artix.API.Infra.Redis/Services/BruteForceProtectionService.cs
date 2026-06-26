namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Logging;

public class BruteForceProtectionService : IBruteForceProtectionService
{
    private readonly IRequestRatePolicy _rateLimiter;
    private readonly ILogger<BruteForceProtectionService> _logger;

    // تنظیمات قابل تنظیم
    private readonly int _maxAttempts = 5; // حداکثر 5 تلاش
    private readonly int _windowMinutes = 15; // در 15 دقیقه
    private readonly int _initialLockoutMinutes = 5; // قفل اولیه 5 دقیقه
    private readonly int _maxLockoutHours = 24; // حداکثر قفل 24 ساعت

    public BruteForceProtectionService(
        IRequestRatePolicy rateLimiter,
        ILogger<BruteForceProtectionService> logger)
    {
        _rateLimiter = rateLimiter;
        _logger = logger;
    }

    public async Task<BruteForceCheckResult> CheckAsync(
        string identifier,
        string ipAddress,
        CancellationToken ct = default)
    {
        var key = GetLockoutKey(identifier, ipAddress);
        var attemptsKey = GetAttemptsKey(identifier, ipAddress);

        try
        {
            // بررسی آیا در حالت قفل است
            var lockoutUntil = await GetLockoutTimeAsync(key, ct);

            if (lockoutUntil.HasValue && lockoutUntil.Value > DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "Brute force protection: Blocked access for {Identifier} from {Ip}. Locked until {LockoutUntil}",
                    identifier, ipAddress, lockoutUntil.Value);

                var totalAttempts = await GetTotalAttemptsAsync(attemptsKey, ct);

                return BruteForceCheckResult.Blocked(lockoutUntil.Value, totalAttempts);
            }

            // اگر قفل منقضی شده بود، پاکش کن
            if (lockoutUntil.HasValue && lockoutUntil.Value <= DateTime.UtcNow)
            {
                await ResetLockoutAsync(key, ct);
            }

            // شمارش تلاش‌های اخیر
            var recentAttempts = await GetRecentAttemptsCountAsync(attemptsKey, ct);
            var remainingAttempts = Math.Max(0, _maxAttempts - recentAttempts);

            return BruteForceCheckResult.Allowed(remainingAttempts, recentAttempts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking brute force for {Identifier} from {Ip}", identifier, ipAddress);
            // در صورت خطا، اجازه بده (fail open)
            return BruteForceCheckResult.Allowed(_maxAttempts, 0);
        }
    }

    public async Task RecordFailedAttemptAsync(
        string identifier,
        string ipAddress,
        CancellationToken ct = default)
    {
        var attemptsKey = GetAttemptsKey(identifier, ipAddress);
        var lockoutKey = GetLockoutKey(identifier, ipAddress);

        try
        {
            // ثبت تلاش ناموفق با timestamp
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var attemptValue = $"{timestamp}|failed";

            await _rateLimiter.RecordAttemptAsync(attemptsKey, attemptValue, TimeSpan.FromMinutes(_windowMinutes));

            // شمارش تعداد تلاش‌ها در پنجره زمانی
            var attemptCount = await GetRecentAttemptsCountAsync(attemptsKey, ct);

            _logger.LogInformation(
                "Brute force: Failed attempt {AttemptCount}/{MaxAttempts} for {Identifier} from {Ip}",
                attemptCount, _maxAttempts, identifier, ipAddress);

            // اگر از حداکثر بیشتر شد، اعمال قفل
            if (attemptCount >= _maxAttempts)
            {
                var lockoutMinutes = CalculateLockoutDuration(attemptCount);
                var lockoutUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);

                await SetLockoutAsync(lockoutKey, lockoutUntil, ct);

                _logger.LogWarning(
                    "Brute force: Account locked for {Identifier} from {Ip} until {LockoutUntil}. Attempts: {AttemptCount}",
                    identifier, ipAddress, lockoutUntil, attemptCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording failed attempt for {Identifier} from {Ip}", identifier, ipAddress);
        }
    }

    public async Task RecordSuccessAsync(
        string identifier,
        string ipAddress,
        CancellationToken ct = default)
    {
        var attemptsKey = GetAttemptsKey(identifier, ipAddress);
        var lockoutKey = GetLockoutKey(identifier, ipAddress);

        try
        {
            // پاک کردن رکوردهای تلاش ناموفق
            await ResetAsync(identifier, ipAddress, ct);

            // ثبت تلاش موفق برای تحلیل
            var successKey = $"bf_success:{identifier}:{ipAddress}";
            await _rateLimiter.RecordAttemptAsync(successKey, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                TimeSpan.FromDays(30));

            _logger.LogInformation("Brute force: Successful login for {Identifier} from {Ip}", identifier, ipAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording success for {Identifier} from {Ip}", identifier, ipAddress);
        }
    }

    public async Task ResetAsync(
        string identifier,
        string ipAddress,
        CancellationToken ct = default)
    {
        var attemptsKey = GetAttemptsKey(identifier, ipAddress);
        var lockoutKey = GetLockoutKey(identifier, ipAddress);

        try
        {
            await _rateLimiter.ResetAsync(attemptsKey);
            await _rateLimiter.ResetAsync(lockoutKey);

            _logger.LogDebug("Brute force protection reset for {Identifier} from {Ip}", identifier, ipAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting brute force for {Identifier} from {Ip}", identifier, ipAddress);
        }
    }

    public async Task<Dictionary<string, int>> GetStatsAsync(
        string identifier,
        CancellationToken ct = default)
    {
        var stats = new Dictionary<string, int>();

        try
        {
            // آمار برای IPهای مختلف این کاربر
            var pattern = $"bf_attempts:{identifier}:*";
            // این نیاز به متد جدید در IRequestRatePolicy دارد
            // فعلاً یک نمونه ساده برمی‌گردانیم
            stats["recent_attempts"] = 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stats for {Identifier}", identifier);
        }

        return stats;
    }

    // ============= Private Methods =============

    private string GetAttemptsKey(string identifier, string ipAddress)
        => $"bf_attempts:{identifier}:{ipAddress}";

    private string GetLockoutKey(string identifier, string ipAddress)
        => $"bf_lockout:{identifier}:{ipAddress}";

    private async Task<int> GetRecentAttemptsCountAsync(string key, CancellationToken ct)
    {
        // در IRequestRatePolicy باید متد GetRecentCountAsync وجود داشته باشد
        // فعلاً یک مقدار پیش‌فرض برمی‌گردانیم
        return await _rateLimiter.GetAttemptCountAsync(key, ct);
    }

    private async Task<int> GetTotalAttemptsAsync(string key, CancellationToken ct)
    {
        return await _rateLimiter.GetTotalCountAsync(key, ct);
    }

    private async Task<DateTime?> GetLockoutTimeAsync(string key, CancellationToken ct)
    {
        var value = await _rateLimiter.GetStringAsync(key, ct);
        if (DateTime.TryParse(value, out var lockoutTime))
        {
            return lockoutTime;
        }

        return null;
    }

    private async Task SetLockoutAsync(string key, DateTime lockoutUntil, CancellationToken ct)
    {
        await _rateLimiter.SetStringAsync(key, lockoutUntil.ToString("O"), lockoutUntil - DateTime.UtcNow, ct);
    }

    private async Task ResetLockoutAsync(string key, CancellationToken ct)
    {
        await _rateLimiter.ResetAsync(key, ct);
    }

    private int CalculateLockoutDuration(int attemptCount)
    {
        // افزایش تصاعدی زمان قفل: 5, 10, 20, 40 دقیقه و الی آخر
        var excess = attemptCount - _maxAttempts;
        var minutes = _initialLockoutMinutes * Math.Pow(2, excess);
        return (int)Math.Min(minutes, _maxLockoutHours * 60);
    }
}
