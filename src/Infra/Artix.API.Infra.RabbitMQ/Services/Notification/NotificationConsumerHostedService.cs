namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System.Text.Json;
using Core.Contract.Primitives.Infra.RabbitMQ;
using Core.Domain.Entities.Notification.Enums;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sql.Data.DbContexts;

internal sealed class NotificationConsumerHostedService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationConsumerHostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private IChannel? _channel;

    public NotificationConsumerHostedService(
        IConnection connection,
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationConsumerHostedService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _connection = connection;
        _hubContext = hubContext;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await DeclareTopologyAsync(stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.Span;
                var message = JsonSerializer.Deserialize<NotificationMessage>(body);

                if (message == null)
                {
                    _logger.LogWarning("Received null notification message. Acking anyway. DeliveryTag: {Tag}",
                        ea.DeliveryTag);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                    return;
                }

                _logger.LogInformation("Received notification {NotificationId} for User {UserId} via {RoutingKey}",
                    message.NotificationId, message.UserId, ea.RoutingKey);


                if (ea.RoutingKey == "notifications.all")
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, stoppingToken);
                }
                else if (ea.RoutingKey.StartsWith("notifications.user."))
                {
                    var userIdStr = ea.RoutingKey["notifications.user.".Length..];
                    await _hubContext.Clients.Group(userIdStr).SendAsync("ReceiveNotification", message, stoppingToken);
                }

                await TryMarkAsDeliveredAsync(message.NotificationId, stoppingToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                _logger.LogDebug("Notification {NotificationId} processed and acked.", message.NotificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process notification message. Nacking. DeliveryTag: {Tag}",
                    ea.DeliveryTag);
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync("all_notifications", false, consumer, stoppingToken);
        await _channel.BasicConsumeAsync("user_notifications", false, consumer, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task DeclareTopologyAsync(CancellationToken ct)
    {
        await _channel.ExchangeDeclareAsync("notifications", ExchangeType.Topic, durable: true, cancellationToken: ct);

        await _channel.QueueDeclareAsync("all_notifications", durable: true, exclusive: false, autoDelete: false,
            cancellationToken: ct);
        await _channel.QueueDeclareAsync("user_notifications", durable: true, exclusive: false, autoDelete: false,
            cancellationToken: ct);

        await _channel.QueueBindAsync("all_notifications", "notifications", "notifications.all", cancellationToken: ct);
        await _channel.QueueBindAsync("user_notifications", "notifications", "notifications.user.*",
            cancellationToken: ct);
    }

    private async Task TryMarkAsDeliveredAsync(Guid notificationId, CancellationToken ct)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();

            var notification = await db.Notifications
                .FirstOrDefaultAsync(n => n.BusinessId == notificationId, ct);

            if (notification == null)
            {
                _logger.LogWarning("Notification {NotificationId} not found in DB. Possibly already processed.", notificationId);
                return;
            }

            if (notification.Status == NotificationStatus.Sent)
            {
                _logger.LogDebug("Notification {NotificationId} already marked as Sent.", notificationId);
                return;
            }

            notification.MarkAsSent();
            await db.SaveChangesAsync(ct);
            _logger.LogInformation("Notification {NotificationId} marked as Sent in DB.", notificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notification {NotificationId} as delivered", notificationId);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            try
            {
                await _channel.CloseAsync(cancellationToken);
            }
            catch
            {
            }

            _channel.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }
}
