namespace Artix.API.Webservice1;

using Core.ApplicationService;
using Core.Contract;
using Core.Contract.Configs.Elasticsearch;
using Endpoints;
using Infra.Sql;
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

        services.AddApplicationServices();
        services.AddContractServices();


        services.AddElasticsearch(configuration);
        services.AddCorsPolicy(configuration);
        services.AddSqlServices(configuration);
        // TODO: put job infra DI here
        services.AddControllers();
    }
}
