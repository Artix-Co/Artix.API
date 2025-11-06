namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Configs.Redis;
using Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

public sealed class RedisConnectionFactory : IRedisConnectionFactory
{
    private readonly Lazy<IConnectionMultiplexer> _connection;
    private readonly ILogger<RedisConnectionFactory> _logger;

    public RedisConnectionFactory(IOptions<RedisOptions> options, ILogger<RedisConnectionFactory> logger)
    {
        _logger = logger;
        _connection = new Lazy<IConnectionMultiplexer>(() =>
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { { options.Value.Host, options.Value.Port } },
                Password = options.Value.Password,
                AbortOnConnectFail = false,
                ConnectRetry = 3,
                ConnectTimeout = 5000,
                SyncTimeout = 5000
            };

            _logger.LogInformation("Connecting to Redis {Host}:{Port}", options.Value.Host, options.Value.Port);
            var muxer = ConnectionMultiplexer.Connect(config);
            muxer.ConnectionFailed += (sender, args) =>
                _logger.LogError(args.Exception, "Redis connection failed: {FailureType}", args.FailureType);
            muxer.ConnectionRestored += (sender, args) =>
                _logger.LogWarning("Redis connection restored after {FailureType}", args.FailureType);

            return muxer;
        });
    }

    public IConnectionMultiplexer Connection => _connection.Value;
}
