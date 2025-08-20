namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System.Text.Json;
using Core.Domain.Entities.Notification;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Models.Notification;
using Microsoft.Extensions.DependencyInjection;
using Sql.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

public class NotificationConsumerHostedService : BackgroundService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory; // اضافه

    public NotificationConsumerHostedService(
        RabbitMqConnectionFactory factory,
        IHubContext<NotificationHub> hubContext,
        IServiceScopeFactory scopeFactory)
    {
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
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
                await _hubContext.Clients.Group(message.UserId.ToString())
                    .SendAsync("ReceiveNotification", message, stoppingToken);
            }
            else if (ea.RoutingKey == "notifications.all")
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, stoppingToken);
            }

            // اضافه: آپدیت status
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
            var notification = await dbContext.Notifications
                .Include(n=>n.UserNotifications)
                .FirstOrDefaultAsync(n => n.BusinessId == message.NotificationId, stoppingToken);
            if (notification != null)
            {
                notification.MarkAsSent();
                await dbContext.SaveChangesAsync(stoppingToken);
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
