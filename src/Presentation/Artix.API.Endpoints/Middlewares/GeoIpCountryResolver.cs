namespace Artix.API.Endpoints.Middlewares;

using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Artix.API.Core.Contract.Configs.GeoIp;
using Microsoft.Extensions.Options;

internal sealed class GeoIpCountryResolver(
    HttpClient httpClient,
    IOptions<GeoIpSettings> options) : IGeoIpCountryResolver
{
    private readonly GeoIpSettings _settings = options.Value;

    public async Task<string?> ResolveAsync(string ip, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.BaseUrl))
            return null;

        if (!Uri.TryCreate(_settings.BaseUrl, UriKind.Absolute, out var baseUri) ||
            !string.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            return null;

        var path = _settings.LookupPathTemplate.Replace("{ip}", Uri.EscapeDataString(ip), StringComparison.Ordinal);
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUri, path));
        if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(_settings.TimeoutSeconds, 1, 10)));
        using var response = await httpClient.SendAsync(request, timeout.Token);
        if (!response.IsSuccessStatusCode)
            return null;

        await using var content = await response.Content.ReadAsStreamAsync(timeout.Token);
        var result = await JsonSerializer.DeserializeAsync<GeoIpResponse>(content, cancellationToken: timeout.Token);
        return string.IsNullOrWhiteSpace(result?.CountryCode)
            ? null
            : result.CountryCode.Trim().ToUpperInvariant();
    }

    private sealed record GeoIpResponse([property: JsonPropertyName("countryCode")] string? CountryCode);
}
