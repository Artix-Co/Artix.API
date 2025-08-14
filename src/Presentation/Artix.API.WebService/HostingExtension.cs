namespace Artix.API.WebService;

using Core.ApplicationService;
using Core.Contract;
using Core.Contract.Configs.Authentication;
using Core.Contract.Configs.AuthenticationApi;
using Core.Contract.Configs.Elasticsearch;
using Core.Contract.Configs.FileSettings;
using Core.Contract.Configs.RabbitMQ;
using Core.DomainService;
using Endpoints;
using Filters;
using Infra.File;
using Infra.Identity;
using Infra.RabbitMQ;
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
        services.Configure<ElasticsearchSettings>(configuration.GetSection("Elasticsearch"));
        services.Configure<FileSettings>(configuration.GetSection("FileSettings"));
        services.Configure<AuthenticationApiSettings>(configuration.GetSection("AuthenticationApi"));
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMqOptions"));


        // Configure Elasticsearch
        var elasticSearchSection = configuration.GetSection("Elasticsearch").Get<ElasticsearchSettings>();


        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchSection.Uri))
            {
                AutoRegisterTemplate = true,
                IndexFormat = elasticSearchSection.IndexFormat,
                ModifyConnectionSettings = c => c
                    .BasicAuthentication(elasticSearchSection.Username, elasticSearchSection.Password)
                    .RequestTimeout(TimeSpan.FromMinutes(elasticSearchSection.RequestTimeoutInMinutes))
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
        services.AddResponseCaching();
        
        
        services.AddRabbitMqService();
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
}
