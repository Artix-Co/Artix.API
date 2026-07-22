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
using Microsoft.AspNetCore.ResponseCompression;
using Utils;
using Artix.API.WebService.Extensions;

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
    public static void AddArtixServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes
                .Concat(["application/json", "application/octet-stream", "application/wasm"]);
        });
        services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Optimal);
        services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Optimal);

        services.Configure<AuthenticationSettings>(configuration.GetSection("Authentication"));
        services.Configure<ElasticsearchSettings>(configuration.GetSection("Elasticsearch"));
        services.Configure<FileSettings>(configuration.GetSection("FileSettings"));
        services.Configure<AuthenticationApiSettings>(configuration.GetSection("AuthenticationApi"));
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMqOptions"));
        services.Configure<RedisOptions>(configuration.GetSection("RedisOptions"));
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        ValidateRequiredSettings(configuration);

        services.AddElasticsearch(configuration);

        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        services.AddHttpClient<CustomerApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://external-api.example.com/");
                client.Timeout = Timeout.InfiniteTimeSpan;
            })
            .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
            .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy())
            .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy());

        services.AddMemoryCache();
        services.AddResponseCaching();

        services.AddRabbitMqService();
        services.AddIdentityService(configuration);
        services.AddRedis();
        services.AddFileService();

        services.AddApplicationServices();
        services.AddContractServices();

        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:3000", "https://localhost:3000"];

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
        });

        services.AddSqlServices(configuration);
        services.AddMongoServices(configuration);
        // After infra registrations so readiness checks reuse shared clients.
        services.AddArtixHealthChecks();
        services.AddDomainServiceServices();
        services.AddEndpointsServices();
    }

    private static void ValidateRequiredSettings(IConfiguration configuration)
    {
        static void Require(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(
                    $"Missing required configuration '{name}'. Set it via environment variables or appsettings.");
        }

        Require("ConnectionStrings:CommandConnectionString",
            configuration.GetConnectionString("CommandConnectionString"));
        Require("ConnectionStrings:QueryConnectionString",
            configuration.GetConnectionString("QueryConnectionString"));
        Require("Authentication:IssuerSigningKey",
            configuration["Authentication:IssuerSigningKey"]);
        Require("RedisOptions:Password", configuration["RedisOptions:Password"]);
        Require("RabbitMqOptions:Password", configuration["RabbitMqOptions:Password"]);
        Require("MongoDbSettings:ConnectionString", configuration["MongoDbSettings:ConnectionString"]);
    }
}
