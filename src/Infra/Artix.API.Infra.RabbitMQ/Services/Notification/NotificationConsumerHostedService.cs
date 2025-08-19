namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System.Text.Json;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Models.Notification;

public class NotificationConsumerHostedService : BackgroundService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public NotificationConsumerHostedService(
        RabbitMqConnectionFactory factory,
        IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        // تعریف exchange
        _channel.ExchangeDeclareAsync("notifications", ExchangeType.Topic, durable: true);

        // تعریف queue برای کاربران
        _channel.QueueDeclareAsync("user_notifications", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBindAsync("user_notifications", "notifications", "notifications.user.*");

        // تعریف queue برای broadcast
        _channel.QueueDeclareAsync("all_notifications", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBindAsync("all_notifications", "notifications", "notifications.all");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<NotificationMessage>(body);
            if (message == null) return;

            if (ea.RoutingKey.StartsWith("notifications.user."))
            {
                // ارسال به کاربر خاص (گروه UserId)
                await _hubContext.Clients.Group(message.UserId.ToString())
                    .SendAsync("ReceiveNotification", message, stoppingToken);
            }
            else if (ea.RoutingKey == "notifications.all")
            {
                // ارسال به همه
                await _hubContext.Clients.All
                    .SendAsync("ReceiveNotification", message, stoppingToken);
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };

        await _channel.BasicConsumeAsync(queue: "user_notifications", autoAck: false, consumer: consumer,
            cancellationToken: stoppingToken);
        await _channel.BasicConsumeAsync(queue: "all_notifications", autoAck: false, consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.CloseAsync();
        _connection.CloseAsync();
        base.Dispose();
    }
}
