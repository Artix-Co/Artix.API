namespace Artix.API.Infra.Identity;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Contract.Configs.Authentication;
using Core.Contract.Primitives.Infra.Identity;
using Core.Contract.Primitives.Infra.Identity.Authentication;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.Entities.User;
using Core.Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

public static class DependencyInjection
{
    public static void AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDeviceFingerprintService, DeviceFingerprintService>();


        services
            .AddIdentity<AppUser, AppRole>(options =>
            {
                // -------------------------------------------------
                // User
                // -------------------------------------------------

                options.User.RequireUniqueEmail = false;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

                // -------------------------------------------------
                // SignIn
                // -------------------------------------------------

                options.SignIn.RequireConfirmedAccount = false;

                options.SignIn.RequireConfirmedEmail = false;

                options.SignIn.RequireConfirmedPhoneNumber = true;

                // -------------------------------------------------
                // Password
                // فقط برای Admin استفاده می‌شود
                // -------------------------------------------------

                options.Password.RequireDigit = true;

                options.Password.RequireLowercase = true;

                options.Password.RequireUppercase = true;

                options.Password.RequireNonAlphanumeric = true;

                options.Password.RequiredLength = 14;

                options.Password.RequiredUniqueChars = 8;

                // -------------------------------------------------
                // Lockout
                // -------------------------------------------------

                options.Lockout.AllowedForNewUsers = true;

                options.Lockout.MaxFailedAccessAttempts = 5;

                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(30);

                // -------------------------------------------------
                // Claims
                // -------------------------------------------------

                options.ClaimsIdentity.UserIdClaimType =
                    ClaimTypes.NameIdentifier;

                options.ClaimsIdentity.UserNameClaimType =
                    ClaimTypes.Name;

                options.ClaimsIdentity.RoleClaimType =
                    ClaimTypes.Role;

                options.ClaimsIdentity.SecurityStampClaimType =
                    "sst";

                // -------------------------------------------------
                // Tokens
                // -------------------------------------------------

                options.Tokens.AuthenticatorIssuer = "Artix";

                options.Tokens.ChangeEmailTokenProvider =
                    TokenOptions.DefaultProvider;

                options.Tokens.ChangePhoneNumberTokenProvider =
                    TokenOptions.DefaultPhoneProvider;

                options.Tokens.PasswordResetTokenProvider =
                    TokenOptions.DefaultProvider;
            })
            .AddEntityFrameworkStores<ArtixCommandDbContext>()
            .AddDefaultTokenProviders();


        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var authSettings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
                authSettings = ValidateAuthenticationSettings(authSettings);


                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(authSettings.IssuerSigningKey));

                options.RequireHttpsMetadata = true;

                options.SaveToken = false;

                options.IncludeErrorDetails = false;

                options.RefreshOnIssuerKeyNotFound = true;

                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authSettings.Audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateActor = false,
                    ValidateTokenReplay = false,
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrWhiteSpace(context.Token))
                        {
                            context.NoResult();
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var services = context.HttpContext.RequestServices;

                        // var principal = context.Principal;
                        // if (principal is null)
                        // {
                        //     context.Fail("Unauthorized.");
                        //     return;
                        // }

                        // ----------------------------------------------------
                        // Claims
                        // ----------------------------------------------------

                        // var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                        // var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti);

                        // if (userIdClaim is null || jtiClaim is null)
                        // {
                        //     context.Fail("Invalid token.");
                        //     return;
                        // }
                        //
                        // if (!long.TryParse(userIdClaim.Value, out var userId))
                        // {
                        //     context.Fail("Invalid user.");
                        //     return;
                        // }

                        // ----------------------------------------------------
                        // User
                        // ----------------------------------------------------

                        // var userManager = services.GetRequiredService<UserManager<AppUser>>();

                        // var user = await userManager.FindByIdAsync(userId.ToString());

                        // if (user is null)
                        // {
                        //     context.Fail("User not found.");
                        //     return;
                        // }
                        //
                        // if (user.IsBanned)
                        // {
                        //     context.Fail("User is banned.");
                        //     return;
                        // }

                        // ----------------------------------------------------
                        // SecurityStamp
                        // ----------------------------------------------------

                        // var tokenStamp = principal.FindFirst("sst")?.Value;

                        // if (string.IsNullOrWhiteSpace(tokenStamp))
                        // {
                        //     context.Fail("Security stamp missing.");
                        //     return;
                        // }

                        // var currentStamp = await userManager.GetSecurityStampAsync(user);

                        // if (!string.Equals(
                        //         tokenStamp,
                        //         currentStamp,
                        //         StringComparison.Ordinal))
                        // {
                        //     context.Fail("Security stamp mismatch.");
                        //     return;
                        // }
                        // ----------------------------------------------------
                        // Revocation
                        // ----------------------------------------------------

                        // var revocationStore =
                        //     services.GetRequiredService<ITokenRevocationStore>();

                        // if (await revocationStore.IsRevokedAsync(jtiClaim.Value))
                        // {
                        //     context.Fail("Token revoked.");
                        //     return;
                        // }

                        // ----------------------------------------------------
                        // Session
                        // ----------------------------------------------------

                        // var sessionService =
                        //     services.GetRequiredService<IUserSessionService>();

                        // var sessionValid =
                        //     await sessionService.IsValidSessionAsync(
                        //         userId,
                        //         jtiClaim.Value,
                        //         context.HttpContext.RequestAborted);

                        // if (!sessionValid)
                        // {
                        //     context.Fail("Session expired.");
                        //     return;
                        // }

                        // ----------------------------------------------------
                        // Fingerprint
                        // ----------------------------------------------------

                        // var fingerprintService =
                        //     services.GetRequiredService<IDeviceFingerprintService>();
                        //
                        // var expected =
                        //     fingerprintService.Create(context.HttpContext);
                        //
                        // var actual =
                        //     principal.FindFirst("fingerprint")?.Value;

                        // if (string.IsNullOrWhiteSpace(actual))
                        // {
                        //     context.Fail("Fingerprint missing.");
                        //     return;
                        // }
                        //
                        // if (!string.Equals(
                        //         tokenStamp,
                        //         currentStamp,
                        //         StringComparison.Ordinal))
                        // {
                        //     context.Fail("Security stamp mismatch.");
                        //     return;
                        // }
                    },
                    // OnAuthenticationFailed = context =>
                    // {
                    //     context.NoResult();
                    //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //     return Task.CompletedTask;
                    // },
                    // OnChallenge = context =>
                    // {
                    //     context.HandleResponse();
                    //
                    //     context.Response.StatusCode = 401;
                    //
                    //     return context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                    // },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;

                        return context.Response.WriteAsJsonAsync(new { error = "Forbidden" });
                    }
                };
            });

        services
            .AddOptions<AuthenticationSettings>()
            .Bind(configuration.GetSection("Authentication"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpContextAccessor();

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<
            IPasswordHasher<AppUser>,
            PasswordHasher<AppUser>>();

        services.AddSingleton<ILookupNormalizer,
            UpperInvariantLookupNormalizer>();


        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    private static AuthenticationSettings ValidateAuthenticationSettings(AuthenticationSettings? settings)
    {
        if (settings == null ||
            string.IsNullOrEmpty(settings.Issuer) ||
            string.IsNullOrEmpty(settings.Audience) ||
            string.IsNullOrEmpty(settings.IssuerSigningKey))
        {
            throw new InvalidOperationException(
                "Authentication configuration (Issuer, Audience, or IssuerSigningKey) is missing or invalid.");
        }

        return settings;
    }
}
