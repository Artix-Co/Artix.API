namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System.Text.Json;
using Artix.API.Infra.RabbitMQ.Models.Notification;
using Services;
using Sql.Data.DbContexts;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal sealed class NotificationConsumerHostedService : BackgroundService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public NotificationConsumerHostedService(
        RabbitMqConnectionFactory factory,
        IHubContext<NotificationHub> hubContext,
        IServiceScopeFactory scopeFactory)
    {
        this._hubContext = hubContext;
        this._scopeFactory = scopeFactory;
        this._connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        this._channel = this._connection.CreateChannelAsync().GetAwaiter().GetResult();

        // تعریف exchange
        this._channel.ExchangeDeclareAsync("notifications", ExchangeType.Topic, durable: true);

        // تعریف queue برای کاربران
        this._channel.QueueDeclareAsync("user_notifications", durable: true, exclusive: false, autoDelete: false);
        this._channel.QueueBindAsync("user_notifications", "notifications", "notifications.user.*");

        // تعریف queue برای broadcast
        this._channel.QueueDeclareAsync("all_notifications", durable: true, exclusive: false, autoDelete: false);
        this._channel.QueueBindAsync("all_notifications", "notifications", "notifications.all");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(this._channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<NotificationMessage>(body);
            if (message == null) return;

            if (ea.RoutingKey.StartsWith("notifications.user."))
            {
                await this._hubContext.Clients.Group(message.UserId.ToString())
                    .SendAsync("ReceiveNotification", message, cancellationToken);
            }
            else if (ea.RoutingKey == "notifications.all")
            {
                await this._hubContext.Clients.All.SendAsync("ReceiveNotification", message, cancellationToken);
            }

            using var scope = this._scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
            var notification = await dbContext.Notifications
                .Include(n => n.UserNotifications)
                .FirstOrDefaultAsync(n => n.BusinessId == message.NotificationId, cancellationToken);
            if (notification != null)
            {
                notification.MarkAsSent();
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            await this._channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
        };

        await this._channel.BasicConsumeAsync(queue: "user_notifications", autoAck: false, consumer: consumer,
            cancellationToken);
        await this._channel.BasicConsumeAsync(queue: "all_notifications", autoAck: false, consumer: consumer,
            cancellationToken);

        await Task.CompletedTask;
    }


    public override void Dispose()
    {
        this._channel.CloseAsync();
        this._connection.CloseAsync();
        base.Dispose();
    }
}
