namespace Artix.API.WebService.Extensions;

using Core.Contract.Configs.Elasticsearch;
using Elastic.Transport;
using Nest;

public static class ElasticsearchExtensions
{
    public static ElasticsearchStatus AddElasticsearch(this IServiceCollection services, IConfiguration config)
    {
        var elastic = config.GetSection("Elasticsearch").Get<ElasticsearchSettings>();
        var resolvedIndex = string.Format(elastic.IndexFormat, DateTime.UtcNow);

        var settings = new ConnectionSettings(new Uri(elastic.Uri))
            .DefaultIndex(resolvedIndex)
            .BasicAuthentication(elastic.Username, elastic.Password)
            .RequestTimeout(TimeSpan.FromMinutes(elastic.RequestTimeoutInMinutes))
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        var client = new ElasticClient(settings);
        services.AddSingleton<IElasticClient>(client);

        var ping = client.Ping();

        var status = new ElasticsearchStatus
        {
            IsValid = ping.IsValid,
            Uri = elastic.Uri,
            Index = resolvedIndex,
            Settings = elastic
        };

        // این خط حیاتیه → وضعیت رو به DI اضافه کن
        services.AddSingleton(status);

        return status;
    }
}

public sealed class ElasticsearchStatus
{
    public bool IsValid { get; set; }
    public string Uri { get; set; } = default!;
    public string Index { get; set; } = default!;
    public ElasticsearchSettings Settings { get; set; } = default!;
}

