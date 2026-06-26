namespace Artix.API.Endpoints.Middlewares;

using Core.Contract.Primitives.Infra.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class GeoFencingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GeoFencingMiddleware> _logger;
    
    // کشورهای مجاز (لیست سفید)
    private readonly HashSet<string> _allowedCountries = new()
    {
        "IR",  // ایران
        "AE",  // امارات
        "TR",  // ترکیه
        "OM",  // عمان
        "QA"   // قطر
    };
    
    // کشورهای ممنوع (لیست سیاه - اولویت بالاتر)
    private readonly HashSet<string> _blockedCountries = new()
    {
        "XX", "YY" // کشورهای مسدود شده
    };
    
    // APIهای حساس که نیاز به محدودیت جغرافیایی دارند
    private readonly string[] _sensitivePaths = new[]
    {
        "/api/payment",
        "/api/withdraw",
        "/api/admin",
        "/api/settings"
    };
    
    public GeoFencingMiddleware(
        RequestDelegate next,
        ILogger<GeoFencingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // بررسی آیا endpoint نیاز به محدودیت جغرافیایی دارد
        if (RequiresGeoFencing(context))
        {
            using var scope = context.RequestServices.CreateScope();
            var rateLimiter = scope.ServiceProvider.GetRequiredService<IRequestRatePolicy>();
            
            var ip = GetClientIp(context);
            var countryCode = await GetCountryCodeAsync(ip, rateLimiter);
            
            // بررسی لیست سیاه
            if (_blockedCountries.Contains(countryCode))
            {
                _logger.LogWarning(
                    "Geo-fencing: Blocked access from blocked country {Country} for IP {Ip} to {Path}",
                    countryCode, ip, context.Request.Path);
                
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Access from your country is not allowed",
                    countryCode = countryCode
                });
                return;
            }
            
            // بررسی لیست سفید برای APIهای حساس
            if (IsSensitivePath(context.Request.Path) && !_allowedCountries.Contains(countryCode))
            {
                _logger.LogWarning(
                    "Geo-fencing: Access to sensitive API from non-allowed country {Country} for IP {Ip} to {Path}",
                    countryCode, ip, context.Request.Path);
                
                // برای APIهای حساس، حتی اگر در لیست سفید نیست، لاگ می‌کنیم ولی اجازه می‌دهیم
                // (می‌توانید به جای آن 403 برگردانید)
                
                // اگر می‌خواهید مسدود کنید:
                /*
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Access to this resource from your country is restricted",
                    countryCode = countryCode
                });
                return;
                */
            }
            
            // اضافه کردن country code به Header برای استفاده در لایه‌های بالاتر
            context.Response.Headers["X-Country-Code"] = countryCode;
        }
        
        await _next(context);
    }
    
    private bool RequiresGeoFencing(HttpContext context)
    {
        // همیشه برای APIهای حساس چک کن
        if (IsSensitivePath(context.Request.Path))
            return true;
        
        // برای سایر APIها، فقط اگر کاربر احراز هویت شده باشد
        return context.User.Identity?.IsAuthenticated == true;
    }
    
    private bool IsSensitivePath(PathString path)
    {
        return _sensitivePaths.Any(p => path.Value?.StartsWith(p, StringComparison.OrdinalIgnoreCase) == true);
    }
    
    private async Task<string> GetCountryCodeAsync(string ip, IRequestRatePolicy rateLimiter)
    {
        if (string.IsNullOrEmpty(ip) || ip == "unknown" || ip == "::1")
            return "LOCAL";
        
        var cacheKey = $"geo:{ip}";
        
        try
        {
            // بررسی کش
            var cached = await rateLimiter.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
                return cached;
            
            // برای محیط توسعه/تست
            if (ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip.StartsWith("172."))
                return "LOCAL";
            
            // در محیط واقعی، از یک سرویس GeoIP استفاده کنید
            // مثال: استفاده از freegeoip.app یا MaxMind
            var countryCode = await GetCountryCodeFromServiceAsync(ip);
            
            // ذخیره در کش به مدت 24 ساعت
            await rateLimiter.SetStringAsync(cacheKey, countryCode, TimeSpan.FromHours(24));
            
            return countryCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting country code for IP {Ip}", ip);
            return "UNKNOWN";
        }
    }
    
    private async Task<string> GetCountryCodeFromServiceAsync(string ip)
    {
        // روش 1: استفاده از API رایگان (با احتیاط - محدودیت دارد)
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(3);
            
            var response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ip}?fields=countryCode");
            // Parse JSON response
            if (response.Contains("\"countryCode\":\""))
            {
                var start = response.IndexOf("\"countryCode\":\"") + 15;
                var end = response.IndexOf("\"", start);
                return response.Substring(start, end - start);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get country from external service for IP {Ip}", ip);
        }
        
        // روش 2: استفاده از MaxMind GeoLite2 (توصیه شده)
        // var country = await _geoIpService.GetCountryAsync(ip);
        // return country.Code;
        
        return "UNKNOWN";
    }
    
    private string GetClientIp(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
            return cfIp.ToString();
        
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.FirstOrDefault()?.Split(',').FirstOrDefault() ?? "unknown";
        
        if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
            return realIp.ToString();
        
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
