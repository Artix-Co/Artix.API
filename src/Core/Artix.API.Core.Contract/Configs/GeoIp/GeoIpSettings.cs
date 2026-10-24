namespace Artix.API.Core.Contract.Configs.GeoIp;

public sealed class GeoIpSettings
{
    public bool Enabled { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string LookupPathTemplate { get; set; } = "/{ip}";
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 2;
}
