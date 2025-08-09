namespace Artix.API.Infra.Identity;

using System.Security.Claims;
using System.Text;
using Core.Contract.Configs.Authentication;
using Core.Domain.Entities.User;
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
        // Configure Identity
        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ArtixCommandDbContext>()
            .AddDefaultTokenProviders();

        // Configure Authentication
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
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.IssuerSigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager =
                            context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                        var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                          context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                        Console.WriteLine($"User ID Claim: {userIdClaim}");

                        if (string.IsNullOrEmpty(userIdClaim))
                        {
                            context.Fail("Unauthorized: User ID claim missing.");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userIdClaim);
                        if (user == null)
                        {
                            context.Fail($"Unauthorized: User not found for ID {userIdClaim}.");
                            return;
                        }

                        // Get the raw token from the Authorization header
                        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
                            !authHeader.ToString().StartsWith("Bearer "))
                        {
                            context.Fail("Unauthorized: Bearer token missing or invalid.");
                            return;
                        }

                        var tokenString = authHeader.ToString().Substring("Bearer ".Length).Trim();
                        Console.WriteLine($"Presented Token: {tokenString}");

                        var storedToken =
                            await userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "access_token");
                        Console.WriteLine($"Stored Token: {storedToken}");

                        if (string.IsNullOrEmpty(storedToken))
                        {
                            context.Fail($"Unauthorized: No token found for user {userIdClaim}.");
                            return;
                        }

                        if (storedToken != tokenString)
                        {
                            context.Fail($"Unauthorized: Token has been revoked for user {userIdClaim}.");
                            return;
                        }
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        // Configure Authorization and Other Services
        services.AddAuthorization();
        
        
        
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
