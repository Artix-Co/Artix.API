namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Configs.Redis;
using Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

public sealed class RedisConnectionFactory : IRedisConnectionFactory
{
    private readonly Lazy<IConnectionMultiplexer> _connection;

    public RedisConnectionFactory(IOptions<RedisOptions> options)
    {
        _connection = new Lazy<IConnectionMultiplexer>(() =>
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { { options.Value.Host, options.Value.Port } },
                Password = options.Value.Password,
                AbortOnConnectFail = false
            };
            return ConnectionMultiplexer.Connect(config);
        });
    }

    public IConnectionMultiplexer Connection => _connection.Value;
}
