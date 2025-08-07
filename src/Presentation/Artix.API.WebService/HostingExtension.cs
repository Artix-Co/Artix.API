namespace Artix.API.WebService;

using System.Security.Claims;
using System.Text;
using Core.ApplicationService;
using Core.Contract;
using Core.Contract.Configs.Authentication;
using Core.Contract.Configs.Elasticsearch;
using Core.Domain.Entities.User;
using Core.DomainService;
using Endpoints;
using Filters;
using Infra.File;
using Infra.Sql;
using Infra.Sql.Data;
using Infra.Sql.Data.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Serilog;
using Serilog.Sinks.Elasticsearch;

public static class HostingExtension
{
    private static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticsearchSettings = configuration.GetSection("Elasticsearch").Get<ElasticsearchSettings>();


        var resolvedIndexName = string.Format(elasticsearchSettings.IndexFormat, DateTime.UtcNow);

        var settings = new ConnectionSettings(new Uri(elasticsearchSettings.Uri))
            .DefaultIndex(resolvedIndexName)
            .BasicAuthentication(elasticsearchSettings.Username, elasticsearchSettings.Password)
            .RequestTimeout(TimeSpan.FromMinutes(elasticsearchSettings.RequestTimeoutInMinutes))
            .EnableDebugMode();


        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);

        services.AddResponseCompression(options => { options.EnableForHttps = true; });
    }


    public static void AddArtixServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Authentication Settings
        services.Configure<AuthenticationSettings>(configuration.GetSection("Authentication"));

        // Configure Elasticsearch
        var elasticSettings = configuration.GetSection("Elasticsearch").Get<ElasticsearchSettings>();
        ValidateElasticsearchSettings(elasticSettings);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSettings.Uri))
            {
                AutoRegisterTemplate = true,
                IndexFormat = elasticSettings.IndexFormat,
                ModifyConnectionSettings = c => c
                    .BasicAuthentication(elasticSettings.Username, elasticSettings.Password)
                    .RequestTimeout(TimeSpan.FromMinutes(elasticSettings.RequestTimeoutInMinutes))
            })
            .CreateLogger();

        // Configure HSTS
        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });


        // Configure Cache
        services.AddMemoryCache();


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
        services.AddApplicationServices();
        services.AddContractServices();
        services.AddElasticsearch(configuration);
        services.AddCorsPolicy(configuration);
        services.AddSqlServices(configuration);
        services.AddFileService(configuration);
        services.AddDomainServiceServices();
        
        services.AddControllers();

        services.AddSwaggerGen(options =>
        {
        

            // Define the Bearer authentication scheme in Swagger
            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field"
                });


            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }

    private static void ValidateElasticsearchSettings(ElasticsearchSettings settings)
    {
        if (settings == null ||
            string.IsNullOrEmpty(settings.Uri) ||
            string.IsNullOrEmpty(settings.Username) ||
            string.IsNullOrEmpty(settings.Password) ||
            string.IsNullOrEmpty(settings.IndexFormat) ||
            settings.RequestTimeoutInMinutes <= 0)
        {
            throw new InvalidOperationException("Elasticsearch configuration is missing or invalid.");
        }
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


