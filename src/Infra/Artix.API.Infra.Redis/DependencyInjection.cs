namespace Artix.API.Infra.Redis;

using Core.Contract.Configs.Redis;
using Core.Contract.Features.Caches.Museums;
using Core.Contract.Features.Caches.Objects;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class DependencyInjection
{
    public static void AddRedis(this IServiceCollection services)
    {
       
        // Only register IConnectionMultiplexer if not already registered
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            var redisConfig = new ConfigurationOptions
            {
                EndPoints = { { redisOptions.Host, redisOptions.Port } },
                Password = redisOptions.Password,
                AbortOnConnectFail = false
            };

            return ConnectionMultiplexer.Connect(redisConfig);
        });

        // Register RedisCacheService for RecentMuseumDto
        services.AddSingleton<ICacheService<RecentMuseumDto>>(sp =>
            new RedisCacheService<RecentMuseumDto>(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                "recent:museums:user",
                sp.GetRequiredService<ILogger<RedisCacheService<RecentMuseumDto>>>()));

        // Register RedisCacheService for RecentObjectDto
        services.AddSingleton<ICacheService<RecentObjectDto>>(sp =>
            new RedisCacheService<RecentObjectDto>(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                "recent:objects:user",
                sp.GetRequiredService<ILogger<RedisCacheService<RecentObjectDto>>>()));
    }
}

 
