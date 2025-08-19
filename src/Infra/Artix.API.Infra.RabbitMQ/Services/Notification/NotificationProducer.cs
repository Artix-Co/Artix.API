namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System.Text.Json;
using global::RabbitMQ.Client;
using Interfaces.Notification;

public class NotificationProducer : INotificationProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private bool _disposed = false;

    public NotificationProducer(RabbitMqConnectionFactory factory)
    {
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.ExchangeDeclareAsync("notifications", ExchangeType.Topic, durable: true).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message,
        CancellationToken cancellationToken = default)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

        await _channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: true, // اگر پیام غیرقابل روتینگ باشه، برگردانده می‌شه
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
    }
    

    public void Dispose()
    {
        this._connection.Dispose();
        this._channel.Dispose();
    }
}
