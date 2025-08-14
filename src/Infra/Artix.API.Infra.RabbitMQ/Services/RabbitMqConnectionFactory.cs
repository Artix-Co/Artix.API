namespace Artix.API.Infra.RabbitMQ.Services;

using Core.Contract.Configs.RabbitMQ;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Options;

public class RabbitMqConnectionFactory
{
    private readonly ConnectionFactory _factory;

    public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options)
    {
        _factory = new ConnectionFactory
        {
            HostName = options.Value.Host,
            Port = options.Value.Port,
            UserName = options.Value.Username,
            Password = options.Value.Password,
            DispatchConsumersAsync = true
        };
    }

    public IConnection CreateConnection()
    {
        return _factory.CreateConnection();
    }
}
