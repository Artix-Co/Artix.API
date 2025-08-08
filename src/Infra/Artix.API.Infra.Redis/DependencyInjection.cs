namespace Artix.API.Infra.Redis;

using Core.Contract.Features.Caches;
using Core.Contract.Features.Caches.Museums;
using Core.Contract.Features.Caches.Objects;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;
using StackExchange.Redis;

public static class DependencyInjection
{
    public static void AddRedis(this IServiceCollection services)
    {
        var redisConfig = ConfigurationOptions.Parse("localhost:6379,password=Heli@ghar771379");
        redisConfig.AbortOnConnectFail = false;

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));
        services.AddSingleton<ICacheService<RecentMuseumDto>>(sp =>
            new RedisCacheService<RecentMuseumDto>(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                "recent:museums:user",
                sp.GetRequiredService<ILogger<RedisCacheService<RecentMuseumDto>>>()));
        services.AddSingleton<ICacheService<RecentObjectDto>>(sp =>
            new RedisCacheService<RecentObjectDto>(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                "recent:objects:user",
                sp.GetRequiredService<ILogger<RedisCacheService<RecentObjectDto>>>()));
    }
}
