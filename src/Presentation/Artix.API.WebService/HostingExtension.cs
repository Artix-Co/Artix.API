namespace Artix.API.Webservice1;

using System.Text;
using Core.ApplicationService;
using Core.Contract;
using Core.Contract.Configs.Elasticsearch;
using Core.Domain.Entities.User;
using Endpoints;
using Infra.Sql;
using Infra.Sql.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
            .RequestTimeout(TimeSpan.FromMinutes(int.Parse(elasticsearchSettings.RequestTimeoutInMinutes)))
            .EnableDebugMode();


        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);

        services.AddResponseCompression(options => { options.EnableForHttps = true; });
    }


    public static void AddFinBridgeServices(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticUri = configuration["Elasticsearch:Uri"];
        var username = configuration["Elasticsearch:Username"];
        var password = configuration["Elasticsearch:Password"];
        var indexFormat = configuration["Elasticsearch:IndexFormat"];
        var requestTimeout = configuration["Elasticsearch:RequestTimeoutInMinutes"];
        var requestInMinutes = int.Parse(requestTimeout);


        Log.Logger = new LoggerConfiguration()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
            {
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat,
                ModifyConnectionSettings = c => c.BasicAuthentication(username, password)
                    .RequestTimeout(TimeSpan.FromMinutes(requestInMinutes))
            })
            .ReadFrom.Configuration(configuration)
            .CreateLogger();


        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();


        services.AddOpenApi();

        services.AddIdentity<AppUser, IdentityRole<long>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                // add more options if needed
            })
            .AddEntityFrameworkStores<ArtixCommandDbContext>()
            .AddDefaultTokenProviders();

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
                    ValidIssuer = "your-app",
                    ValidAudience = "your-app",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-super-secret-key"))
                };
            });


        services.AddAuthorization();

        services.AddApplicationServices();
        services.AddContractServices();


        services.AddElasticsearch(configuration);
        services.AddCorsPolicy(configuration);
        services.AddSqlServices(configuration);
        // TODO: put job infra DI here
        services.AddControllers();
    }
}
