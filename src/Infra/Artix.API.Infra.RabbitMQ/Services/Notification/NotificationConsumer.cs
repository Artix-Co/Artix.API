namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class NotificationConsumer : BackgroundService, INotificationConsumer
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSerializer _serializer;
    private readonly INotificationHandler _handler;
    private const string QueueName = "notifications.queue";
    private IConnection _connection;
    private IChannel _channel;

    public NotificationConsumer(IServiceScopeFactory scopeFactory, IMessageSerializer serializer,
        INotificationHandler handler)
    {
        _scopeFactory = scopeFactory;
        _serializer = serializer;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = this._scopeFactory.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<RabbitMqConnectionFactory>();
        this._connection = await factory.CreateConnectionAsync();
        this._channel = await this._connection.CreateChannelAsync(null, cancellationToken);

        await this._channel.ExchangeDeclareAsync("notifications.exchange", ExchangeType.Topic, durable: true,
            cancellationToken: cancellationToken);

        await this._channel.QueueDeclareAsync(queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );

        await this._channel.QueueBindAsync(queue: QueueName,
            exchange: "notifications.exchange",
            routingKey: "notifications.#",
            cancellationToken: cancellationToken
        );

        await this._channel.BasicQosAsync(0, 10, false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(this._channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = this._serializer.Deserialize<NotificationMessage>(body);
                await this._handler.HandleAsync(message);
                await this._channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            }
            catch
            {
                await this._channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
            }
        };

        await this._channel.BasicConsumeAsync(QueueName, false, consumer, cancellationToken);
    }

    public override void Dispose()
    {
        _channel.CloseAsync();
        _channel.Dispose();
        _connection.CloseAsync();
        _connection.Dispose();
        base.Dispose();
    }
}
