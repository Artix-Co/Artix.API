namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Core.Contract.Primitives.Infra.RabbitMQ;
using Core.Domain.Entities.Notification.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sql.Data.DbContexts;

internal sealed class NotificationOutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly INotificationProducer _producer;
    private readonly ILogger<NotificationOutboxProcessor> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public NotificationOutboxProcessor(
        IServiceScopeFactory scopeFactory,
        INotificationProducer producer,
        ILogger<NotificationOutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _producer = producer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();

            var pendingNotifications = await db.Notifications
                .Include(n => n.UserNotifications)
                .Where(n =>
                    n.Status == NotificationStatus.Pending &&
                    (n.ExpirationDate == null || n.ExpirationDate > DateTime.UtcNow))
                .OrderByDescending(n => n.Priority)
                .ThenBy(n => n.CreatedAt)
                .Take(100)
                .ToListAsync(stoppingToken);

            if (!pendingNotifications.Any())
            {
                await Task.Delay(_interval, stoppingToken);
                continue;
            }

            foreach (var notification in pendingNotifications)
            {
                try
                {
                    if (notification.IsBroadcast)
                    {
                        var message = new NotificationMessage(
                            notification.BusinessId,
                            null,
                            notification.Title,
                            notification.Body,
                            notification.Type,
                            notification.CreatedAt,
                            notification.Metadata);

                        await _producer.PublishAsync("notifications", "notifications.all", message, stoppingToken);
                        notification.MarkAsSent();
                    }
                    else
                    {
                        var sentToAny = false;

                        foreach (var un in notification.UserNotifications)
                        {
                            if (un.UserId <= 0)
                            {
                                _logger.LogWarning("Skipping invalid UserId {UserId} for notification {NotificationId}", un.UserId, notification.BusinessId);
                                continue;
                            }

                            try
                            {
                                var message = new NotificationMessage(
                                    notification.BusinessId,
                                    un.UserId,
                                    notification.Title,
                                    notification.Body,
                                    notification.Type,
                                    notification.CreatedAt,
                                    notification.Metadata);

                                await _producer.PublishAsync("notifications", $"notifications.user.{un.UserId}", message, stoppingToken);
                                sentToAny = true;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to send notification to UserId {UserId}. Skipping.", un.UserId);
                                // ادامه بده، بقیه کاربران رو نفرست
                            }
                        }

                        // اگه حداقل به یک نفر فرستاده شد → MarkAsSent
                        if (sentToAny || !notification.UserNotifications.Any())
                        {
                            notification.MarkAsSent();
                        }
                        else
                        {
                            notification.MarkAsFailed("No valid recipients or all publishes failed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    notification.MarkAsFailed($"Publish failed: {ex.Message}");
                    _logger.LogError(ex, "Failed to process notification {NotificationId}", notification.BusinessId);
                }
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(_interval, stoppingToken);
        }
    }
}
