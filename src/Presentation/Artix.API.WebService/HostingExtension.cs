namespace Artix.API.WebService;

using Core.ApplicationService;
using Core.Contract;
using Core.Contract.Configs.Authentication;
using Core.Contract.Configs.Elasticsearch;
using Core.Contract.Configs.FileSettings;
using Core.DomainService;
using Endpoints;
using Filters;
using Infra.File;
using Infra.Identity;
using Infra.Redis;
using Infra.Sql;
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

        
        
        var section = configuration.GetSection("FileSettings");
        var options = section.Get<FileSettings>();

        if (options == null || string.IsNullOrWhiteSpace(options.StoragePath))
        {
            throw new ArgumentException("FileSettings:StoragePath configuration is missing or empty.", nameof(configuration));
        }

        services.Configure<FileSettings>(section);

        
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

        services.AddIdentityService(configuration);

        services.AddFileService();

        services.AddRedis();
        
        services.AddApplicationServices();
        services.AddContractServices();
        services.AddElasticsearch(configuration);
        services.AddCorsPolicy(configuration);
        services.AddSqlServices(configuration);
        
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

  
}


