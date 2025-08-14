namespace Artix.API.Infra.Identity;

using System.Security.Claims;
using System.Text;
using Core.Contract.Configs.Authentication;
using Core.Domain.Entities.User;
using Core.Domain.Entities.User.Enums;
using Interfaces.LoginHistory;
using Interfaces.TokenProvider;
using Interfaces.TokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Services.LoginHistory;
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
                    ClockSkew = TimeSpan.Zero // بی‌تاخیر
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager =
                            context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
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

                        // در این نسخه چک Access Token ذخیره شده حذف شده
                        // JWT به تنهایی اعتبارسنجی می‌شود
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
                policy.RequireClaim("ClientType", nameof(ClientType.Emerald), nameof(ClientType.Ruby), nameof(ClientType.Turquoise), nameof(ClientType.Pro)));
        });

        services.AddScoped<IUserLoginHistoryService, UserLoginHistoryService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ITokenService, TokenService>();
    }

    private static void ValidateAuthenticationSettings(AuthenticationSettings settings)
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
