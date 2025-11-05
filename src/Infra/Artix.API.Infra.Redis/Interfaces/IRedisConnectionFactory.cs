namespace Artix.API.Infra.Redis.Interfaces;

using StackExchange.Redis;

public interface IRedisConnectionFactory
{
    IConnectionMultiplexer Connection { get; }
}
