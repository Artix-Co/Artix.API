namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System;
using System.Threading.Tasks;
using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Logging;

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
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        try
        {
            this._logger.LogInformation("Initializing NotificationProducer, creating RabbitMQ connection.");
            this._connection = factory.CreateConnection();
            this._channel = this._connection.CreateModel();
            this._channel.ExchangeDeclare(this._exchangeName, ExchangeType.Topic, durable: true);
            this._logger.LogInformation("Successfully declared exchange: {ExchangeName}", this._exchangeName);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to initialize NotificationProducer with exchange: {ExchangeName}", this._exchangeName);
            throw;
        }
    }

    public Task PublishAsync(NotificationMessage message, string routingKey)
    {
        if (message == null)
        {
            this._logger.LogWarning("Attempted to publish null NotificationMessage.");
            throw new ArgumentNullException(nameof(message));
        }

        if (string.IsNullOrEmpty(routingKey))
        {
            this._logger.LogWarning("Attempted to publish with empty or null routing key for message ID: {MessageId}", message.NotificationId);
            throw new ArgumentException("Routing key cannot be null or empty.", nameof(routingKey));
        }

        try
        {
            this._logger.LogDebug("Serializing message ID: {MessageId} with routing key: {RoutingKey}", message.NotificationId, routingKey);
            var body = this._serializer.Serialize(message);
            var props = this._channel.CreateBasicProperties();
            props.Persistent = true;
            props.MessageId = message.NotificationId.ToString();
            props.Timestamp = new AmqpTimestamp(new DateTimeOffset(message.CreatedAt).ToUnixTimeSeconds());

            this._logger.LogInformation("Publishing message ID: {MessageId} to exchange: {ExchangeName} with routing key: {RoutingKey}", 
                message.NotificationId, this._exchangeName, routingKey);
            this._channel.BasicPublish(this._exchangeName, routingKey, props, body.ToArray());
            this._logger.LogDebug("Successfully published message ID: {MessageId}", message.NotificationId);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to publish message ID: {MessageId} to exchange: {ExchangeName} with routing key: {RoutingKey}", 
                message.NotificationId, this._exchangeName, routingKey);
            throw;
        }
    }

    public void Dispose()
    {
        if (this._disposed)
        {
            this._logger.LogDebug("NotificationProducer already disposed.");
            return;
        }

        try
        {
            this._logger.LogInformation("Disposing NotificationProducer, closing channel and connection.");
            this._channel?.Close();
            this._channel?.Dispose();
            this._connection?.Close();
            this._connection?.Dispose();
            this._disposed = true;
            this._logger.LogInformation("NotificationProducer disposed successfully.");
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error occurred while disposing NotificationProducer.");
        }
    }
}
