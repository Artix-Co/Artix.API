namespace Artix.API.Core.Contract.Configs.Elasticsearch;


public class ElasticsearchSettings
{
    public string Uri { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string IndexFormat { get; set; }
    public int RequestTimeoutInMinutes { get; set; }
}
