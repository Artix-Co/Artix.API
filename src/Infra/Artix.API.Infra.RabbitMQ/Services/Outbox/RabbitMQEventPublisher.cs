namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
using Core.Contract.Configs.RabbitMQ;
using Core.Domain.DomainEvents;
using global::RabbitMQ.Client;
using Interfaces.Outbox;
using Microsoft.Extensions.Options;

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchange;

    public RabbitMQEventPublisher(RabbitMqConnectionFactory connectionFactory, IOptions<RabbitMqOptions> options)
    {
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _exchange = "domain-events";
        _channel.ExchangeDeclare(_exchange, ExchangeType.Topic, durable: true);
    }

    public async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var eventType = @event.GetType().Name;
        var routingKey = $"domain.{eventType}";
        var messageBody = JsonSerializer.SerializeToUtf8Bytes(@event);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        await Task.Run(() =>
        {
            _channel.BasicPublish(
                exchange: _exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: messageBody);
        }, cancellationToken);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
