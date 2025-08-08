namespace Artix.API.Core.DomainService;

using Microsoft.Extensions.DependencyInjection;
using Users;
using Users.LoginHistory;
using Users.Token;

public static class DependencyInjection
{
    public static void AddDomainServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IUserLoginHistoryService, UserLoginHistoryService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ITokenService, TokenService>();
    }
}
