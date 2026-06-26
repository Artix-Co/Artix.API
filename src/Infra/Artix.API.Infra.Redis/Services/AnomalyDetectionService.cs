namespace Artix.API.Infra.Redis.Services;

using System.Text.Json;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class AnomalyDetectionService : IAnomalyDetectionService
{
    private readonly IRequestRatePolicy _rateLimiter;
    private readonly ILogger<AnomalyDetectionService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // تنظیمات
    private readonly double _anomalyThreshold = 0.7; // بالای 70% ناهنجار محسوب می‌شود
    private readonly int _historyDays = 30; // نگهداری 30 روز تاریخچه

    public AnomalyDetectionService(
        IRequestRatePolicy rateLimiter,
        ILogger<AnomalyDetectionService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _rateLimiter = rateLimiter;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AnomalyDetectionResult> DetectAsync(
        long userId,
        string action,
        HttpContext context,
        CancellationToken ct = default)
    {
        var result = new AnomalyDetectionResult();
        var anomalies = new List<string>();
        var score = 0.0;

        try
        {
            var ip = GetClientIp(context);
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var deviceId = context.Request.Headers["X-Device-Id"].ToString();
            var currentTime = DateTime.UtcNow;

            // 1. بررسی Impossible Travel (سفر غیرممکن)
            var travelAnomaly = await DetectImpossibleTravelAsync(userId, ip, currentTime, ct);
            if (travelAnomaly.HasValue)
            {
                anomalies.Add($"Impossible travel detected: {travelAnomaly.Value.reason}");
                score += 0.4;
                result.Metadata["previous_location"] = travelAnomaly.Value.previousLocation;
                result.Metadata["current_location"] = travelAnomaly.Value.currentLocation;
                result.Metadata["time_difference_hours"] = travelAnomaly.Value.timeDifferenceHours;
            }

            // 2. بررسی Device تغییر کرده
            var deviceAnomaly = await DetectNewDeviceAsync(userId, deviceId, ct);
            if (deviceAnomaly)
            {
                anomalies.Add("New device detected");
                score += 0.25;
            }

            // 3. بررسی زمان غیرعادی
            var timeAnomaly = await DetectUnusualTimeAsync(userId, currentTime, ct);
            if (timeAnomaly)
            {
                anomalies.Add("Unusual login/action time");
                score += 0.15;
            }

            // 4. بررسی نرخ غیرعادی درخواست‌ها
            var rateAnomaly = await DetectAbnormalRateAsync(userId, action, ct);
            if (rateAnomaly.HasValue)
            {
                anomalies.Add($"Abnormal request rate: {rateAnomaly.Value} requests/sec");
                score += 0.2;
            }

            // 5. بررسی تغییر الگوی UserAgent
            var userAgentAnomaly = await DetectUserAgentChangeAsync(userId, userAgent, ct);
            if (userAgentAnomaly)
            {
                anomalies.Add("UserAgent changed significantly");
                score += 0.1;
            }

            // 6. بررسی حساس بودن اکشن
            var actionSensitivity = GetActionSensitivity(action);
            score = Math.Min(1.0, score + actionSensitivity);

            // تعیین اکشن مورد نیاز بر اساس نمره
            result.IsAnomalous = score >= _anomalyThreshold;
            result.AnomalyScore = score;
            result.DetectedAnomalies = anomalies;

            if (result.IsAnomalous)
            {
                result.RequiredAction = DetermineRequiredAction(score, anomalies);

                _logger.LogWarning(
                    "Anomaly detected for user {UserId}. Action: {Action}, Score: {Score:F2}, Anomalies: {Anomalies}",
                    userId, action, score, string.Join(", ", anomalies));

                // ذخیره رویداد ناهنجار برای تحلیل بعدی
                await StoreAnomalyEventAsync(userId, action, result, ct);
            }
            else if (score > 0.3)
            {
                _logger.LogDebug(
                    "Suspicious behavior for user {UserId}. Score: {Score:F2}, Anomalies: {Anomalies}",
                    userId, score, string.Join(", ", anomalies));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting anomaly for user {UserId}", userId);
            // در صورت خطا، نرمال فرض کن
            return AnomalyDetectionResult.Normal();
        }

        return result;
    }

    public async Task<AnomalyScore> GetUserTrustScoreAsync(long userId, CancellationToken ct = default)
    {
        var score = new AnomalyScore();

        try
        {
            // بازیابی معیارهای مختلف از Redis
            var deviceTrust = await GetDeviceTrustScoreAsync(userId, ct);
            var locationTrust = await GetLocationTrustScoreAsync(userId, ct);
            var behaviorTrust = await GetBehaviorTrustScoreAsync(userId, ct);
            var timeTrust = await GetTimeTrustScoreAsync(userId, ct);

            score.DeviceTrust = deviceTrust;
            score.LocationTrust = locationTrust;
            score.BehaviorTrust = behaviorTrust;
            score.TimeTrust = timeTrust;
            score.OverallScore = (deviceTrust + locationTrust + behaviorTrust + timeTrust) / 4.0;

            score.RiskLevel = score.OverallScore switch
            {
                >= 0.8 => "LOW",
                >= 0.6 => "MEDIUM",
                >= 0.4 => "HIGH",
                _ => "CRITICAL"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trust score for user {UserId}", userId);
            score.OverallScore = 0.5;
            score.RiskLevel = "MEDIUM";
        }

        return score;
    }

    public async Task LogNormalBehaviorAsync(
        long userId,
        string action,
        HttpContext context,
        CancellationToken ct = default)
    {
        try
        {
            var behaviorKey = $"user_behavior:{userId}:{action}";
            var behaviorData = new
            {
                Timestamp = DateTime.UtcNow,
                Ip = GetClientIp(context),
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                DeviceId = context.Request.Headers["X-Device-Id"].ToString(),
                Hour = DateTime.UtcNow.Hour,
                DayOfWeek = DateTime.UtcNow.DayOfWeek
            };

            var json = JsonSerializer.Serialize(behaviorData);
            await _rateLimiter.AddToListAsync(behaviorKey, json, TimeSpan.FromDays(_historyDays), ct);

            // نگهداری حداکثر 1000 رکورد اخیر
            await _rateLimiter.TrimListAsync(behaviorKey, 1000, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging normal behavior for user {UserId}", userId);
        }
    }

    // ============= Private Detection Methods =============

    private async Task<(string previousLocation, string currentLocation, double timeDifferenceHours, string reason)?>
        DetectImpossibleTravelAsync(long userId, string currentIp, DateTime currentTime, CancellationToken ct)
    {
        var lastLoginKey = $"user_last_location:{userId}";
        var lastLoginData = await _rateLimiter.GetStringAsync(lastLoginKey, ct);

        if (string.IsNullOrEmpty(lastLoginData))
            return null;

        try
        {
            var lastData = JsonSerializer.Deserialize<LastLocationData>(lastLoginData);
            if (lastData == null) return null;

            var previousIp = lastData.Ip;
            var previousTime = lastData.Timestamp;
            var timeDifference = (currentTime - previousTime).TotalHours;

            if (timeDifference > 24) // اگر بیش از 24 ساعت گذشته، نادیده بگیر
                return null;

            // محاسبه موقعیت جغرافیایی تقریبی از IP
            var previousLocation = await GetLocationFromIpAsync(previousIp, ct);
            var currentLocation = await GetLocationFromIpAsync(currentIp, ct);

            if (previousLocation == null || currentLocation == null)
                return null;

            // محاسبه فاصله تقریبی (خط مستقیم)
            var distance = CalculateDistance(
                previousLocation.Latitude, previousLocation.Longitude,
                currentLocation.Latitude, currentLocation.Longitude);

            // سرعت مورد نیاز برای این فاصله (کیلومتر بر ساعت)
            var requiredSpeed = distance / timeDifference;

            // حداکثر سرعت منطقی سفر (مثلاً 900 کیلومتر بر ساعت برای هواپیما)
            const double maxPossibleSpeed = 1000;

            if (requiredSpeed > maxPossibleSpeed && timeDifference < 12)
            {
                return (previousLocation.City, currentLocation.City, timeDifference,
                    $"Distance {distance:N0}km in {timeDifference:F1}h requires {requiredSpeed:F0}km/h");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting impossible travel for user {UserId}", userId);
        }

        return null;
    }

    private async Task<bool> DetectNewDeviceAsync(long userId, string deviceId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(deviceId))
            return false;

        var knownDevicesKey = $"user_devices:{userId}";
        var knownDevices = await _rateLimiter.GetSetMembersAsync(knownDevicesKey, ct);

        if (!knownDevices.Any())
        {
            // اولین دستگاه - ثبت کن
            await _rateLimiter.AddToSetAsync(knownDevicesKey, deviceId, TimeSpan.FromDays(_historyDays), ct);
            return false;
        }

        var isNewDevice = !knownDevices.Contains(deviceId);

        if (isNewDevice)
        {
            // ثبت دستگاه جدید
            await _rateLimiter.AddToSetAsync(knownDevicesKey, deviceId, TimeSpan.FromDays(_historyDays), ct);
        }

        return isNewDevice;
    }

    private async Task<bool> DetectUnusualTimeAsync(long userId, DateTime currentTime, CancellationToken ct)
    {
        var behaviorKey = $"user_behavior_time:{userId}";
        var usualHours = await _rateLimiter.GetListRangeAsync(behaviorKey, 0, 99, ct);

        if (!usualHours.Any())
        {
            // ذخیره ساعت فعلی به عنوان مرجع
            await _rateLimiter.AddToListAsync(behaviorKey, currentTime.Hour.ToString(), TimeSpan.FromDays(_historyDays),
                ct);
            return false;
        }

        var usualHoursSet = usualHours.Select(int.Parse).Distinct().ToList();
        var currentHour = currentTime.Hour;

        // اگر ساعت فعلی در ساعات معمول نیست و بین 12 شب تا 5 صبح است
        var isUnusual = !usualHoursSet.Contains(currentHour) && (currentHour < 6 || currentHour > 22);

        if (isUnusual)
        {
            // اضافه کردن به تاریخچه
            await _rateLimiter.AddToListAsync(behaviorKey, currentHour.ToString(), TimeSpan.FromDays(_historyDays), ct);
        }

        return isUnusual;
    }

    private async Task<int?> DetectAbnormalRateAsync(long userId, string action, CancellationToken ct)
    {
        var rateKey = $"user_rate:{userId}:{action}";
        var recentRequests = await _rateLimiter.GetRecentCountInLastMinuteAsync(rateKey, ct);

        // میانگین نرمال: 5 درخواست در دقیقه
        // ناهنجار: بیش از 20 درخواست در دقیقه
        if (recentRequests > 20)
            return recentRequests;

        if (recentRequests > 10)
        {
            _logger.LogDebug("High request rate for user {UserId}: {Count}/min", userId, recentRequests);
        }

        return null;
    }

    private async Task<bool> DetectUserAgentChangeAsync(long userId, string currentUserAgent, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(currentUserAgent))
            return false;

        var userAgentKey = $"user_ua:{userId}";
        var lastUserAgent = await _rateLimiter.GetStringAsync(userAgentKey, ct);

        if (string.IsNullOrEmpty(lastUserAgent))
        {
            await _rateLimiter.SetStringAsync(userAgentKey, currentUserAgent, TimeSpan.FromDays(_historyDays), ct);
            return false;
        }

        // اگر UserAgent کاملاً متفاوت است
        var isDifferent = !IsSimilarUserAgent(lastUserAgent, currentUserAgent);

        if (isDifferent)
        {
            await _rateLimiter.SetStringAsync(userAgentKey, currentUserAgent, TimeSpan.FromDays(_historyDays), ct);
        }

        return isDifferent;
    }

    // ============= Helper Methods =============

    private double GetActionSensitivity(string action)
    {
        return action.ToLower() switch
        {
            "login" => 0.0,
            "view_profile" => 0.05,
            "update_profile" => 0.1,
            "change_password" => 0.2,
            "transfer_money" => 0.3,
            "withdraw" => 0.35,
            "delete_account" => 0.4,
            "admin_action" => 0.5,
            _ => 0.1
        };
    }

    private string DetermineRequiredAction(double score, List<string> anomalies)
    {
        if (score >= 0.9)
            return "admin_review";

        if (score >= 0.8)
            return "email_verification";

        if (score >= 0.7)
            return "2fa";

        return "log_only";
    }

    private string GetClientIp(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.FirstOrDefault()?.Split(',').FirstOrDefault() ?? "unknown";

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private async Task<LocationData?> GetLocationFromIpAsync(string ip, CancellationToken ct)
    {
        // اینجا باید از یک سرویس GeoIP استفاده کنید
        // فعلاً یک مقدار پیش‌فرض برمی‌گردانیم
        return new LocationData { City = "Unknown", Latitude = 0, Longitude = 0 };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = Deg2rad(lat2 - lat1);
        var dLon = Deg2rad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance in km
    }

    private double Deg2rad(double deg) => deg * (Math.PI / 180);

    private bool IsSimilarUserAgent(string ua1, string ua2)
    {
        if (ua1 == ua2) return true;

        // بررسی برند مرورگر
        var browser1 = ExtractBrowser(ua1);
        var browser2 = ExtractBrowser(ua2);

        if (browser1 != browser2) return false;

        // بررسی OS
        var os1 = ExtractOS(ua1);
        var os2 = ExtractOS(ua2);

        return os1 == os2;
    }

    private string ExtractBrowser(string userAgent)
    {
        if (userAgent.Contains("Chrome")) return "Chrome";
        if (userAgent.Contains("Firefox")) return "Firefox";
        if (userAgent.Contains("Safari")) return "Safari";
        if (userAgent.Contains("Edge")) return "Edge";
        return "Other";
    }

    private string ExtractOS(string userAgent)
    {
        if (userAgent.Contains("Windows")) return "Windows";
        if (userAgent.Contains("Mac")) return "Mac";
        if (userAgent.Contains("Linux")) return "Linux";
        if (userAgent.Contains("Android")) return "Android";
        if (userAgent.Contains("iOS")) return "iOS";
        return "Other";
    }

    private async Task StoreAnomalyEventAsync(long userId, string action, AnomalyDetectionResult result,
        CancellationToken ct)
    {
        var eventKey = $"anomaly_events:{userId}";
        var eventData = new
        {
            Timestamp = DateTime.UtcNow,
            Action = action,
            Score = result.AnomalyScore,
            Anomalies = result.DetectedAnomalies,
            RequiredAction = result.RequiredAction
        };

        var json = JsonSerializer.Serialize(eventData);
        await _rateLimiter.AddToListAsync(eventKey, json, TimeSpan.FromDays(90), ct);
    }

    private async Task<double> GetDeviceTrustScoreAsync(long userId, CancellationToken ct)
    {
        var devicesKey = $"user_devices:{userId}";
        var devices = await _rateLimiter.GetSetMembersAsync(devicesKey, ct);

        if (!devices.Any()) return 0.5;

        // اعتماد به دستگاه‌هایی که بیش از 7 روز استفاده شده‌اند
        return Math.Min(1.0, devices.Count / 5.0);
    }

    private async Task<double> GetLocationTrustScoreAsync(long userId, CancellationToken ct)
    {
        var locationsKey = $"user_locations:{userId}";
        var locations = await _rateLimiter.GetSetMembersAsync(locationsKey, ct);

        if (!locations.Any()) return 0.5;

        return Math.Min(1.0, locations.Count / 3.0);
    }

    private async Task<double> GetBehaviorTrustScoreAsync(long userId, CancellationToken ct)
    {
        var behaviorKey = $"user_behavior:{userId}";
        var behaviors = await _rateLimiter.GetListRangeAsync(behaviorKey, 0, 99, ct);

        if (!behaviors.Any()) return 0.5;

        // محاسبه تناسب رفتاری
        return 0.7;
    }

    private async Task<double> GetTimeTrustScoreAsync(long userId, CancellationToken ct)
    {
        var timeKey = $"user_behavior_time:{userId}";
        var times = await _rateLimiter.GetListRangeAsync(timeKey, 0, 99, ct);

        if (!times.Any()) return 0.5;

        return 0.8;
    }

    // ============= Helper Classes =============

    private class LastLocationData
    {
        public string Ip { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    private class LocationData
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
