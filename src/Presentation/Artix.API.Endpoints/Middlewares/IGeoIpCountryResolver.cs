namespace Artix.API.Endpoints.Middlewares;

public interface IGeoIpCountryResolver
{
    Task<string?> ResolveAsync(string ip, CancellationToken cancellationToken = default);
}
