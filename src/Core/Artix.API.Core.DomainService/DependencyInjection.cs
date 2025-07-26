namespace Artix.API.Core.DomainService;

using Microsoft.Extensions.DependencyInjection;
using Users;

public static class DependencyInjection
{
    public static void AddDomainServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}
