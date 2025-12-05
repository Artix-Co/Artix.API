namespace Artix.API.WebService;

using System.IO.Compression;
using Core.ApplicationService;
using Core.Contract;
using Core.Contract.Configs.Authentication;
using Core.Contract.Configs.AuthenticationApi;
using Core.Contract.Configs.Elasticsearch;
using Core.Contract.Configs.FileSettings;
using Core.Contract.Configs.Mongo;
using Core.Contract.Configs.RabbitMQ;
using Core.Contract.Configs.Redis;
using Core.Contract.Primitives.CircuitBreaker;
using Core.DomainService;
using Endpoints;
using Infra.FileService;
using Infra.Identity;
using Infra.Mongo;
using Infra.RabbitMQ;
using Infra.Redis;
using Infra.Sql;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Utils;

public sealed class CustomerApiClient
{
    private readonly HttpClient _http;

    public CustomerApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> PingAsync()
    {
        var response = await _http.GetAsync("ping");
        await response.EnsureSuccessStatusCodeSafeAsync();
        return await response.Content.ReadAsStringAsync();
    }
}

public static class HostingExtension
{
    public static void AddArtixServices(this IServiceCollection services, IConfiguration configuration,
        bool isDevelopmentEnvironment)
    {
        var keyStorePathKeys = isDevelopmentEnvironment
            ? "/Users/mohammadnazari/.aspnet/DataProtection-Keys"
            : "/app/dataprotection-keys";

        services.AddDataProtection()
            .SetApplicationName("Artix")
            .PersistKeysToFileSystem(new DirectoryInfo(keyStorePathKeys));

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes
                .Concat(new[] { "application/json", "application/octet-stream", "application/wasm" });
        });
        services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Optimal);
        services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Optimal);
        services.AddResponseCompression();


        services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });


        services.Configure<AuthenticationSettings>(configuration.GetSection("Authentication"));
        services.Configure<ElasticsearchSettings>(configuration.GetSection("Elasticsearch"));
        services.Configure<FileSettings>(configuration.GetSection("FileSettings"));
        services.Configure<AuthenticationApiSettings>(configuration.GetSection("AuthenticationApi"));
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMqOptions"));
        services.Configure<RedisOptions>(configuration.GetSection("RedisOptions"));
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        // Configure HSTS
        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });


        services.AddHttpClient<CustomerApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://external-api.example.com/");
                client.Timeout = Timeout.InfiniteTimeSpan; // Timeout via Polly
            })
            .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
            .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy())
            .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());


        services.AddMemoryCache();
        services.AddResponseCaching();

        services.AddRabbitMqService();
        services.AddIdentityService(configuration);

        services.AddFileService();

        services.AddRedis();

        services.AddApplicationServices();
        services.AddContractServices();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
        });
        services.AddSqlServices(configuration);
        services.AddMongoServices(configuration);

        services.AddDomainServiceServices();

        services.AddEndpointsServices();
        
    }
}
