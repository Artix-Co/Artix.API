namespace Artix.API.Infra.RabbitMQ.Services;

using Core.Contract.Configs.RabbitMQ;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Options;

public class RabbitMqConnectionFactory
{
    private readonly ConnectionFactory _factory;

    public RabbitMqConnectionFactory()
    {
        _factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "admin",
            Password = "admin",
            DispatchConsumersAsync = true
        };
    }

    public IConnection CreateConnection()
    {
        return _factory.CreateConnection();
    }
}
