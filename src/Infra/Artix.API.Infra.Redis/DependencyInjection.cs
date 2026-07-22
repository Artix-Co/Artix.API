namespace Artix.API.Infra.Redis;

using Core.Contract.Configs.Redis;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            var config = new ConfigurationOptions
            {
                EndPoints = { { options.Host, options.Port } },
                Password = options.Password,
                AbortOnConnectFail = false,
                ConnectRetry = 3,
                ConnectTimeout = 5000,
                SyncTimeout = 5000
            };

            var muxer = ConnectionMultiplexer.Connect(config);
            var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();

            muxer.ConnectionFailed += (sender, args) =>
                logger.LogError(args.Exception, "Redis connection failed: {FailureType}", args.FailureType);

            muxer.ConnectionRestored += (sender, args) =>
                logger.LogWarning("Redis connection restored after {FailureType}", args.FailureType);

            logger.LogInformation("Connected to Redis {Host}:{Port}", options.Host, options.Port);

            return muxer;
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
