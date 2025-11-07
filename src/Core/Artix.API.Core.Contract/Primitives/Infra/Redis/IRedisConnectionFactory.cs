namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

using StackExchange.Redis;

public interface IRedisConnectionFactory
{
    IConnectionMultiplexer Connection { get; }
}
