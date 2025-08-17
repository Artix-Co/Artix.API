namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System;
using System.Threading.Tasks;
using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class NotificationProducer : INotificationProducer, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private readonly IMessageSerializer _serializer;
    private IChannel _channel;
    private readonly string Exchange = "notifications.exchange";
    private readonly ILogger<NotificationProducer> _logger;


    public NotificationProducer(IServiceScopeFactory scopeFactory, IMessageSerializer serializer,
        ILogger<NotificationProducer> logger)
    {
        this._scopeFactory = scopeFactory;
        this._logger = logger;
        this._serializer = serializer;
    }

    public async Task PublishAsync(NotificationMessage message, string routingKey)
    {
        await using var scope = this._scopeFactory.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<RabbitMqConnectionFactory>();
        this._connection = await factory.CreateConnectionAsync();
        this._channel = await this._connection.CreateChannelAsync();


        var body = this._serializer.Serialize(message);


        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var ch = ((AsyncEventingBasicConsumer)sender).Channel;
            var properties = ea.BasicProperties;
            var replyProps = new BasicProperties
            {
                CorrelationId = properties.CorrelationId,
                Persistent = true,
                MessageId = message.NotificationId.ToString(),
                Timestamp = new AmqpTimestamp(new DateTimeOffset(message.CreatedAt).ToUnixTimeSeconds()),
            };


            await _channel.BasicPublishAsync(
                exchange: Exchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: replyProps,
                body: body);

            await ch.BasicAckAsync(ea.DeliveryTag, false);
        };

    }

    public void Dispose()
    {
        _channel.CloseAsync();
        _channel.Dispose();
        _connection.CloseAsync();
        _connection.Dispose();
    }
}
