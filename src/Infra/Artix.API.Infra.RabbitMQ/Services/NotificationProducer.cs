namespace Artix.API.Infra.RabbitMQ.Services;

using global::RabbitMQ.Client;
using Interfaces;
using Models;

public class NotificationProducer : INotificationProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IMessageSerializer _serializer;
    private readonly IModel _channel;
    private readonly string _exchangeName = "notifications.exchange";

    public NotificationProducer(RabbitMqConnectionFactory factory, IMessageSerializer serializer)
    {
        _connection = factory.CreateConnection();
        _serializer = serializer;
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, true);
    }

    public Task PublishAsync(NotificationMessage message, string routingKey)
    {
        var body = _serializer.Serialize(message);
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;
        props.MessageId = message.NotificationId.ToString();
        props.Timestamp = new AmqpTimestamp(new DateTimeOffset(message.CreatedAt).ToUnixTimeSeconds());
        _channel.BasicPublish(_exchangeName, routingKey, props, body.ToArray());
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
