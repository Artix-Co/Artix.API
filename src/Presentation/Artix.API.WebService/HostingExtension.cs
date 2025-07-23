namespace Artix.API.Webservice1;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Core.ApplicationService;
using Core.Contract;
using Core.Contract.Configs.Authentication;
using Core.Contract.Configs.Elasticsearch;
using Core.Domain.Entities.User;
using Endpoints;
using Infra.Sql;
using Infra.Sql.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.IssuerSigningKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager =
                            context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                        var user = await userManager.GetUserAsync(context.Principal);

                        if (user == null)
                        {
                            context.Fail("Unauthorized: User not found.");
                            return;
                        }

                        var token = context.SecurityToken as JwtSecurityToken;
                        var storedToken =
                            await userManager.GetAuthenticationTokenAsync(user, "ArtixApp", "access_token");

                        if (storedToken != token?.RawData)
                        {
                            context.Fail("Unauthorized: Token has been revoked.");
                        }
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
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("api", new OpenApiInfo
            {
                Title = "Artix API",
                Description = "Single-version production API",
            });
            c.EnableAnnotations(); // For Swashbuckle.AspNetCore.Annotations
            // Optional: Add JWT support since you have Microsoft.AspNetCore.Authentication.JwtBearer
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}'",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
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
