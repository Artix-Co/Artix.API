namespace Artix.API.Infra.RabbitMQ.Services;

using global::RabbitMQ.Client;
using Interfaces;
using Models;

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class NotificationProducer : INotificationProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IMessageSerializer _serializer;
    private readonly IModel _channel;
    private readonly string _exchangeName = "notifications.exchange";
    private readonly ILogger<NotificationProducer> _logger;
    private bool _disposed;

    public NotificationProducer(RabbitMqConnectionFactory factory, IMessageSerializer serializer, ILogger<NotificationProducer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        try
        {
            _logger.LogInformation("Initializing NotificationProducer, creating RabbitMQ connection.");
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
            _logger.LogInformation("Successfully declared exchange: {ExchangeName}", _exchangeName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize NotificationProducer with exchange: {ExchangeName}", _exchangeName);
            throw;
        }
    }

    public Task PublishAsync(NotificationMessage message, string routingKey)
    {
        if (message == null)
        {
            _logger.LogWarning("Attempted to publish null NotificationMessage.");
            throw new ArgumentNullException(nameof(message));
        }

        if (string.IsNullOrEmpty(routingKey))
        {
            _logger.LogWarning("Attempted to publish with empty or null routing key for message ID: {MessageId}", message.NotificationId);
            throw new ArgumentException("Routing key cannot be null or empty.", nameof(routingKey));
        }

        try
        {
            _logger.LogDebug("Serializing message ID: {MessageId} with routing key: {RoutingKey}", message.NotificationId, routingKey);
            var body = _serializer.Serialize(message);
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;
            props.MessageId = message.NotificationId.ToString();
            props.Timestamp = new AmqpTimestamp(new DateTimeOffset(message.CreatedAt).ToUnixTimeSeconds());

            _logger.LogInformation("Publishing message ID: {MessageId} to exchange: {ExchangeName} with routing key: {RoutingKey}", 
                message.NotificationId, _exchangeName, routingKey);
            _channel.BasicPublish(_exchangeName, routingKey, props, body.ToArray());
            _logger.LogDebug("Successfully published message ID: {MessageId}", message.NotificationId);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message ID: {MessageId} to exchange: {ExchangeName} with routing key: {RoutingKey}", 
                message.NotificationId, _exchangeName, routingKey);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            _logger.LogDebug("NotificationProducer already disposed.");
            return;
        }

        try
        {
            _logger.LogInformation("Disposing NotificationProducer, closing channel and connection.");
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _disposed = true;
            _logger.LogInformation("NotificationProducer disposed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while disposing NotificationProducer.");
        }
    }
}
