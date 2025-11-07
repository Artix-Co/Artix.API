 
 

namespace Artix.API.Infra.Redis;

using Core.Contract.Configs.Redis;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services;
using StackExchange.Redis;

public static class DependencyInjection
{
    public static void AddRedis(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;

            var redisConfig = new ConfigurationOptions
            {
                EndPoints = { { options.Host, options.Port } },
                Password = options.Password,
                AbortOnConnectFail = false
            };

            return ConnectionMultiplexer.Connect(redisConfig);
        });

        services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
        services.AddSingleton<IDistributedLockService, RedisLockService>();
        services.AddSingleton<IRequestRatePolicy, RedisRateLimiter>();
        services.AddSingleton<IBackgroundJobScheduler, RedisJobQueueService>();
        services.AddScoped(typeof(ICacheRepository<>), typeof(RedisCacheRepository<>));
        services.AddSingleton<ISessionStore, RedisSessionStore>();
        services.AddSingleton<ITokenRevocationStore, RedisTokenRevocationStore>();
        services.AddSingleton<IFeatureToggleService, RedisFeatureToggleService>();
        services.AddSingleton<IEventDeduplicationStore, RedisEventDeduplicationStore>();
        services.AddSingleton<ILeaderboardService, RedisLeaderboardService>();
        services.AddSingleton<IMessageRelayService, RedisMessageRelayService>();
    }
}
