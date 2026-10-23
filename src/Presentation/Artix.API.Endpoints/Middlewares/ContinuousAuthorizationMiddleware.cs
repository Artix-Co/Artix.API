namespace Artix.API.Endpoints.Middlewares;

using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Core.Contract.Primitives.Infra.Redis;
 

public class ContinuousAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ContinuousAuthorizationMiddleware> _logger;
    
    // عملیات پرخطر
    private readonly HashSet<string> _highRiskOperations = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/payment/transfer",
        "/api/payment/withdraw",
        "/api/account/delete",
        "/api/account/change-password",
        "/api/account/update-email"
    };
    
    // عملیات بحرانی
    private readonly HashSet<string> _criticalOperations = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/admin/",
        "/api/settings/security"
    };
    
    public ContinuousAuthorizationMiddleware(
        RequestDelegate next,
        ILogger<ContinuousAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // فقط برای endpoints احراز هویت شده
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }
        
        using var scope = context.RequestServices.CreateScope();
        var rateLimiter = scope.ServiceProvider.GetRequiredService<IRequestRatePolicy>();
        var anomalyDetection = scope.ServiceProvider.GetService<IAnomalyDetectionService>();
        
        var userId = GetUserId(context.User);
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var method = context.Request.Method;
        
        _logger.LogDebug("Continuous auth check for user {UserId} on {Method} {Path}", userId, method, path);
        
        try
        {
            // ==========================================
            // 1. بررسی توکن در Redis (Revocation Check)
            // ==========================================
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
            
            if (!string.IsNullOrEmpty(token))
            {
                var isRevoked = await IsTokenRevokedAsync(token, rateLimiter);
                if (isRevoked)
                {
                    _logger.LogWarning("Token revoked for user {UserId} on {Path}", userId, path);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Token has been revoked. Please login again.",
                        code = "token_revoked"
                    });
                    return;
                }
            }
            
            // ==========================================
            // 2. بررسی سطح اعتماد دستگاه
            // ==========================================
            var trustScore = await GetDeviceTrustScoreAsync(context, rateLimiter);
            var isHighRisk = IsHighRiskOperation(path);
            var isCritical = IsCriticalOperation(path);
            
            var requiredScore = isCritical ? 0.85 : (isHighRisk ? 0.7 : 0.5);
            
            if (trustScore.OverallScore < requiredScore)
            {
                _logger.LogWarning(
                    "Low trust score {Score:F2} (required {Required:F2}) for user {UserId} on {Path}",
                    trustScore.OverallScore, requiredScore, userId, path);
                
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Additional verification required for this operation",
                    requiredAction = DetermineRequiredAction(trustScore.OverallScore, requiredScore),
                    trustScore = trustScore.OverallScore,
                    requiredScore = requiredScore,
                    anomalies = trustScore.Anomalies
                });
                return;
            }
            
            // ==========================================
            // 3. بررسی Least Privilege
            // ==========================================
            var requiredPermissions = GetRequiredPermissions(path, method);
            var userPermissions = await GetUserPermissionsAsync(context.User, rateLimiter);
            
            var missingPermissions = requiredPermissions.Where(p => !userPermissions.Contains(p)).ToList();
            
            if (missingPermissions.Any())
            {
                _logger.LogWarning(
                    "Missing permissions for user {UserId}. Required: {Required}, Missing: {Missing}",
                    userId, string.Join(", ", requiredPermissions), string.Join(", ", missingPermissions));
                
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Insufficient permissions for this operation",
                    missingPermissions = missingPermissions,
                    code = "insufficient_permissions"
                });
                return;
            }
            
            // ==========================================
            // 4. بررسی JWT (بدون استفاده از JwtSecurityTokenHandler)
            // ==========================================
            var isValidToken = await ValidateTokenFromClaimsAsync(context.User);
            if (!isValidToken)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Invalid or expired token",
                    code = "invalid_token"
                });
                return;
            }
            
            // ==========================================
            // 5. بررسی Anomaly Detection
            // ==========================================
            if (anomalyDetection != null && long.TryParse(userId, out var userIdLong))
            {
                var anomalyResult = await anomalyDetection.DetectAsync(
                    userIdLong,
                    GetOperationName(path),
                    context);
                
                if (anomalyResult.IsAnomalous && anomalyResult.AnomalyScore > 0.8)
                {
                    _logger.LogWarning(
                        "Anomaly detected for user {UserId}. Score: {Score:F2}",
                        userId, anomalyResult.AnomalyScore);
                    
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Suspicious activity detected. Operation blocked.",
                        requiredAction = anomalyResult.RequiredAction ?? "contact_support",
                        anomalyScore = anomalyResult.AnomalyScore,
                        code = "anomaly_detected"
                    });
                    return;
                }
            }
            
            // ==========================================
            // 6. به روز رسانی آخرین فعالیت
            // ==========================================
            await UpdateLastActivityAsync(userId, rateLimiter);
            
            _logger.LogDebug("Continuous auth passed for user {UserId} on {Path}. TrustScore: {Score:F2}", 
                userId, path, trustScore.OverallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ContinuousAuthorizationMiddleware for user {UserId} on {Path}", userId, path);
        }
        
        await _next(context);
    }
    
    // ==========================================
    // Token Validation Methods (بدون JwtSecurityTokenHandler)
    // ==========================================
    
    private async Task<bool> IsTokenRevokedAsync(string token, IRequestRatePolicy rateLimiter)
    {
        if (string.IsNullOrEmpty(token))
            return false;
        
        try
        {
            // استخراج JTI از token بدون استفاده از JwtSecurityTokenHandler
            var jti = ExtractJtiFromToken(token);
            if (string.IsNullOrEmpty(jti))
                return false;
            
            var revokedKey = $"revoked_token:{jti}";
            var isRevoked = await rateLimiter.KeyExistsAsync(revokedKey);
            
            return isRevoked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token revocation");
            return false;
        }
    }
    
    private string? ExtractJtiFromToken(string token)
    {
        try
        {
            // روش ساده: split token و decode payload
            var parts = token.Split('.');
            if (parts.Length != 3)
                return null;
            
            var payload = parts[1];
            // پایین آوردن payload به string
            var base64 = payload.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            
            var jsonBytes = Convert.FromBase64String(base64);
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);
            
            // جستجوی ساده برای jti
            var jtiMatch = System.Text.RegularExpressions.Regex.Match(json, "\"jti\"\\s*:\\s*\"([^\"]+)\"");
            if (jtiMatch.Success)
                return jtiMatch.Groups[1].Value;
            
            return null;
        }
        catch
        {
            return null;
        }
    }
    
    private async Task<bool> ValidateTokenFromClaimsAsync(ClaimsPrincipal user)
    {
        // بررسی Exp claim از Claims
        var expClaim = user.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        
        if (!string.IsNullOrEmpty(expClaim) && long.TryParse(expClaim, out var expUnix))
        {
            var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix);
            if (expDate < DateTimeOffset.UtcNow)
            {
                _logger.LogDebug("Token expired at {ExpDate}", expDate);
                return false;
            }
        }
        
        return true;
    }
    
    private string GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? user.FindFirst("sub")?.Value
            ?? "";
    }
    
    // ==========================================
    // Device Trust Score Methods
    // ==========================================
    
    private async Task<DeviceTrustScore> GetDeviceTrustScoreAsync(HttpContext context, IRequestRatePolicy rateLimiter)
    {
        var result = new DeviceTrustScore();
        var deviceId = context.Request.Headers["X-Device-Id"].ToString();
        var ip = GetClientIp(context);
        
        // 1. دستگاه معروف (0.4)
        if (!string.IsNullOrEmpty(deviceId))
        {
            var isTrustedDevice = await IsTrustedDeviceAsync(deviceId, rateLimiter);
            if (isTrustedDevice)
            {
                result.Score += 0.4;
                result.Reasons.Add("Trusted device");
            }
            else
            {
                result.Anomalies.Add("Unknown device");
            }
        }
        else
        {
            result.Anomalies.Add("Missing device ID");
        }
        
        // 2. IP معروف (0.2)
        var isKnownIp = await IsKnownIpAsync(ip, rateLimiter);
        if (isKnownIp)
        {
            result.Score += 0.2;
            result.Reasons.Add("Known IP address");
        }
        else
        {
            result.Anomalies.Add("Unknown IP address");
        }
        
        // 3. بیومتریک (0.2)
        if (context.Request.Headers.ContainsKey("X-Biometric-Validated"))
        {
            var biometricValidated = context.Request.Headers["X-Biometric-Validated"].ToString();
            if (biometricValidated.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                result.Score += 0.2;
                result.Reasons.Add("Biometric validated");
            }
        }
        
        // 4. Location معمول (0.2)
        var isUsualLocation = await IsUsualLocationAsync(context, rateLimiter);
        if (isUsualLocation)
        {
            result.Score += 0.2;
            result.Reasons.Add("Usual location");
        }
        else
        {
            result.Anomalies.Add("Unusual location");
        }
        
        result.OverallScore = Math.Min(1.0, result.Score);
        
        return result;
    }
    
    private async Task<bool> IsTrustedDeviceAsync(string deviceId, IRequestRatePolicy rateLimiter)
    {
        if (string.IsNullOrEmpty(deviceId))
            return false;
        
        var key = $"trusted_device:{deviceId}";
        var value = await rateLimiter.GetStringAsync(key);
        
        return value == "true";
    }
    
    private async Task<bool> IsKnownIpAsync(string ip, IRequestRatePolicy rateLimiter)
    {
        if (string.IsNullOrEmpty(ip) || ip == "unknown" || ip == "::1" || ip == "127.0.0.1")
            return true;
        
        var key = $"known_ip:{ip}";
        var value = await rateLimiter.GetStringAsync(key);
        
        return value == "true";
    }
    
    private async Task<bool> IsUsualLocationAsync(HttpContext context, IRequestRatePolicy rateLimiter)
    {
        var userId = GetUserId(context.User);
        if (string.IsNullOrEmpty(userId))
            return true;
        
        var ip = GetClientIp(context);
        var key = $"user_location:{userId}";
        var usualLocations = await rateLimiter.GetSetMembersAsync(key);
        
        if (!usualLocations.Any())
            return true;
        
        var ipHash = ComputeSimpleHash(ip);
        return usualLocations.Contains(ipHash);
    }
    
    // ==========================================
    // Permission Methods
    // ==========================================
    
    private List<string> GetRequiredPermissions(string path, string method)
    {
        var permissions = new List<string>();
        
        if (path.StartsWith("/api/admin"))
        {
            permissions.Add("admin.access");
            if (method == "DELETE") permissions.Add("admin.delete");
            if (method == "POST") permissions.Add("admin.create");
            if (method == "PUT") permissions.Add("admin.update");
        }
        else if (path.StartsWith("/api/payment"))
        {
            permissions.Add("payment.access");
            if (path.Contains("transfer") || path.Contains("send"))
                permissions.Add("payment.transfer");
            if (path.Contains("withdraw"))
                permissions.Add("payment.withdraw");
        }
        else if (path.StartsWith("/api/account"))
        {
            permissions.Add("account.access");
            if (path.Contains("delete")) permissions.Add("account.delete");
            if (path.Contains("update")) permissions.Add("account.update");
        }
        else
        {
            permissions.Add("authenticated");
        }
        
        return permissions;
    }
    
    private async Task<HashSet<string>> GetUserPermissionsAsync(ClaimsPrincipal user, IRequestRatePolicy rateLimiter)
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // از Claims خواندن
        var roleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role);
        foreach (var roleClaim in roleClaims)
        {
            permissions.Add($"role.{roleClaim.Value}");
        }
        
        // دسترسی‌های پایه بر اساس نقش
        if (user.IsInRole("Admin"))
        {
            permissions.Add("admin.access");
            permissions.Add("payment.access");
            permissions.Add("account.access");
        }
        else if (user.IsInRole("Client"))
        {
            permissions.Add("profile.access");
            permissions.Add("account.access");
            permissions.Add("payment.access");
            permissions.Add("payment.transfer");
        }
        
        permissions.Add("authenticated");
        
        return permissions;
    }
    
    // ==========================================
    // Helper Methods
    // ==========================================
    
    private bool IsHighRiskOperation(string path)
    {
        return _highRiskOperations.Any(op => path.Contains(op, StringComparison.OrdinalIgnoreCase));
    }
    
    private bool IsCriticalOperation(string path)
    {
        return _criticalOperations.Any(op => path.Contains(op, StringComparison.OrdinalIgnoreCase));
    }
    
    private string DetermineRequiredAction(double currentScore, double requiredScore)
    {
        var gap = requiredScore - currentScore;
        
        if (gap > 0.3) return "contact_support";
        if (gap > 0.15) return "email_verification";
        return "2fa";
    }
    
    private string GetOperationName(string path)
    {
        if (path.Contains("transfer")) return "transfer_money";
        if (path.Contains("withdraw")) return "withdraw";
        if (path.Contains("change-password")) return "change_password";
        if (path.Contains("delete")) return "delete_account";
        if (path.Contains("profile")) return "view_profile";
        if (path.Contains("login")) return "login";
        if (path.Contains("admin")) return "admin_action";
        return "unknown";
    }
    
    private string GetClientIp(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
            return cfIp.ToString();
        
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        
        if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
            return realIp.ToString();
        
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
    
    private string ComputeSimpleHash(string input)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash).Substring(0, 16);
    }
    
    private async Task UpdateLastActivityAsync(string userId, IRequestRatePolicy rateLimiter)
    {
        try
        {
            var key = $"user_last_activity:{userId}";
            var now = DateTime.UtcNow.ToString("O");
            await rateLimiter.SetStringAsync(key, now, TimeSpan.FromMinutes(5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last activity for user {UserId}", userId);
        }
    }
    
    // ==========================================
    // Inner Classes
    // ==========================================
    
    private class DeviceTrustScore
    {
        public double Score { get; set; } = 0;
        public double OverallScore { get; set; } = 0;
        public List<string> Reasons { get; set; } = new();
        public List<string> Anomalies { get; set; } = new();
    }
}
