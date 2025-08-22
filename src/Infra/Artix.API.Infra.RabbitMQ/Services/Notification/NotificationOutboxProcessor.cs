namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Core.Domain.Entities.Notification.Enums;
using Interfaces.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Notification;
using Sql.Data.DbContexts;

internal sealed class NotificationOutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly INotificationProducer _producer;

    public NotificationOutboxProcessor(IServiceScopeFactory scopeFactory, INotificationProducer producer)
    {
        _scopeFactory = scopeFactory;
        this._producer = producer;
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();

            var pendingNotifications = await dbContext.Notifications
                .Include(n => n.UserNotifications)
                .Where(n =>
                    n.Status == NotificationStatus.Pending &&
                    (n.ExpirationDate == null || n.ExpirationDate > DateTime.UtcNow)
                )
                .OrderBy(n => n.Priority)
                .ThenBy(n => n.CreatedAt)
                .Take(100)
                .ToListAsync(cancellationToken);

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
                            notification.Metadata
                        );
                        string routingKey = "notifications.all";

                        await _producer.PublishAsync("notifications", routingKey, message, cancellationToken);

                        notification.MarkAsSent();
                    }
                    else
                    {
                        foreach (var userNotification in notification.UserNotifications)
                        {
                            var message = new NotificationMessage(
                                notification.BusinessId,
                                userNotification.UserId,
                                notification.Title,
                                notification.Body,
                                notification.Type,
                                notification.CreatedAt,
                                notification.Metadata
                            );
                            string routingKey = $"notifications.user.{userNotification.UserId}";

                            await _producer.PublishAsync("notifications", routingKey, message, cancellationToken);
                        }

                        notification.MarkAsSent();
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.Message;
                    notification.MarkAsFailed(errorMessage);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
