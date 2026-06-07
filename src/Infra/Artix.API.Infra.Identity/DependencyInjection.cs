namespace Artix.API.Infra.Identity;

using System.Security.Claims;
using System.Text;
using Core.Contract.Configs.Authentication;
using Core.Contract.Primitives.Infra.Identity;
using Core.Contract.Primitives.Infra.Identity.Authentication;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.Entities.User;
using Core.Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.SessionService;
using Services.TokenProvider;
using Services.TokenService;
using Sql.Data.DbContexts;

public static class DependencyInjection
{
    public static void AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ArtixCommandDbContext>()
            .AddDefaultTokenProviders();

        var authSettings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        ValidateAuthenticationSettings(authSettings);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidAudience = authSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authSettings.IssuerSigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "nameid"
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.Request.Path.StartsWithSegments("/notificationHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        // 1. چک Revocation از طریق Redis
                        var revocationStore = context.HttpContext.RequestServices
                            .GetRequiredService<ITokenRevocationStore>();
                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                        if (!string.IsNullOrEmpty(jti) && await revocationStore.IsRevokedAsync(jti))
                        {
                            context.Fail("Token has been revoked.");
                            return;
                        }

                        // 2. دریافت اطلاعات کاربر
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<UserManager<AppUser>>();
                        var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                          context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                        if (string.IsNullOrEmpty(userIdClaim))
                        {
                            context.Fail("Unauthorized: User ID claim missing.");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userIdClaim);
                        if (user == null)
                        {
                            context.Fail("Unauthorized: User not found.");
                            return;
                        }

                        // 3. چک تطابق Session (جدید)
                        var sessionService = context.HttpContext.RequestServices
                            .GetRequiredService<IUserSessionService>();

                        var userIdLong = long.Parse(userIdClaim);
                        var isValidSession = await sessionService.IsValidSessionAsync(userIdLong, jti);

                        if (!isValidSession)
                        {
                            context.Fail("Session is no longer active. Please login again.");
                            return;
                        }

                        // 4. دریافت Session برای چک‌های بعدی (IP, UserAgent)
                        var session = await sessionService.GetActiveSessionByJwtIdAsync(jti);
                        if (session == null)
                        {
                            context.Fail("Session not found.");
                            return;
                        }

                        // 5. اضافه کردن Session به HttpContext برای استفاده در سایر سرویس‌ها
                        context.HttpContext.Items["UserSession"] = session;

                        // 6. چک IP و UserAgent (مرحله 4 - اینجا هم انجام می‌شود)
                        var currentIp = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                        var currentUserAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();

                        // اگر از X-Forwarded-For استفاده می‌کنید
                        if (context.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
                        {
                            currentIp = forwarded.FirstOrDefault()?.Split(',').FirstOrDefault() ?? currentIp;
                        }

                        if (session.IpAddress != currentIp && session.IpAddress != "unknown")
                        {
                            // آپشن 1: فقط لاگ کن و اجازه بده
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<JwtBearerEvents>>();
                            logger.LogWarning(
                                "IP address mismatch for user {UserId}. Session IP: {SessionIp}, Current IP: {CurrentIp}",
                                userIdLong, session.IpAddress, currentIp);

                            // آپشن 2: رد درخواست (امنیت بالاتر)
                            // context.Fail("IP address mismatch. Possible token theft.");
                            // return;
                        }

                        if (session.UserAgent != currentUserAgent && session.UserAgent != "unknown")
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<JwtBearerEvents>>();
                            logger.LogWarning("UserAgent mismatch for user {UserId}", userIdLong);
                        }
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            // Policy for all Clients (regardless of ClientType)
            options.AddPolicy("ClientPolicy", policy =>
                policy.RequireRole(nameof(Role.Client)));

            // Policy for Admin
            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole(nameof(Role.Admin)));

            // Policy for Emerald Clients
            options.AddPolicy("EmeraldClientPolicy", policy =>
                policy.RequireRole(nameof(Role.Client)).RequireClaim("ClientType", nameof(ClientType.Emerald)));

            // Policy for Ruby Clients
            options.AddPolicy("RubyClientPolicy", policy =>
                policy.RequireRole(nameof(Role.Client)).RequireClaim("ClientType", nameof(ClientType.Ruby)));

            // Policy for Turquoise Clients
            options.AddPolicy("TurquoiseClientPolicy", policy =>
                policy.RequireRole(nameof(Role.Client)).RequireClaim("ClientType", nameof(ClientType.Turquoise)));

            // Policy for Pro Clients
            options.AddPolicy("ProClientPolicy", policy =>
                policy.RequireRole(nameof(Role.Client)).RequireClaim("ClientType", nameof(ClientType.Pro)));

            // Optional: Policy for any user with a specific ClientType (if needed)
            options.AddPolicy("AnyClientTypePolicy", policy =>
                policy.RequireClaim("ClientType", nameof(ClientType.Emerald), nameof(ClientType.Ruby),
                    nameof(ClientType.Turquoise), nameof(ClientType.Pro)));
        });

        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    private static void ValidateAuthenticationSettings(AuthenticationSettings? settings)
    {
        if (settings == null ||
            string.IsNullOrEmpty(settings.Issuer) ||
            string.IsNullOrEmpty(settings.Audience) ||
            string.IsNullOrEmpty(settings.IssuerSigningKey))
        {
            throw new InvalidOperationException(
                "Authentication configuration (Issuer, Audience, or IssuerSigningKey) is missing or invalid.");
        }
    }
}
