namespace Artix.API.Infra.Redis.Services;

using Core.Contract.Configs.Redis;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

public sealed class RedisConnectionFactory : IRedisConnectionFactory
{
    private readonly IConnectionMultiplexer _connection;

    public RedisConnectionFactory(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public IConnectionMultiplexer Connection => _connection;
}
