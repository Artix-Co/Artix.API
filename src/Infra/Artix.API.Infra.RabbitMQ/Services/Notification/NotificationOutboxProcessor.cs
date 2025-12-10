namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Core.Contract.Primitives.Infra.RabbitMQ;
using Core.Domain.Entities.Notification.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sql.Data.DbContexts;

internal sealed class NotificationOutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly INotificationProducer _producer;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public NotificationOutboxProcessor(
        IServiceScopeFactory scopeFactory,
        INotificationProducer producer)
    {
        _scopeFactory = scopeFactory;
        _producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();

            var notifications = await db.Notifications
                .Include(n => n.UserNotifications)
                .Where(n => 
                    n.Status == NotificationStatus.Pending &&
                    (n.ExpirationDate == null || n.ExpirationDate > DateTime.UtcNow))
                .OrderByDescending(n => n.Priority)
                .ThenBy(n => n.CreatedAt)
                .Take(100)
                .ToListAsync(stoppingToken);

            if (!notifications.Any())
            {
                await Task.Delay(_interval, stoppingToken);
                continue;
            }

            foreach (var n in notifications)
            {
                try
                {
                    if (n.IsBroadcast)
                    {
                        var msg = new NotificationMessage(
                            n.BusinessId,
                            null,
                            n.Title,
                            n.Body,
                            n.Type,
                            n.CreatedAt,
                            n.Metadata);

                        await _producer.PublishAsync(
                            "notifications",
                            "notifications.all",
                            msg,
                            stoppingToken);

                        n.MarkAsSent();
                    }
                    else
                    {
                        foreach (var un in n.UserNotifications)
                        {
                            var msg = new NotificationMessage(
                                n.BusinessId,
                                un.UserId,
                                n.Title,
                                n.Body,
                                n.Type,
                                n.CreatedAt,
                                n.Metadata);

                            await _producer.PublishAsync(
                                "notifications",
                                $"notifications.user.{un.UserId}",
                                msg,
                                stoppingToken);
                        }

                        n.MarkAsSent();
                    }
                }
                catch (Exception ex)
                {
                    n.MarkAsFailed(ex.Message);
                }
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(_interval, stoppingToken);
        }
    }
}
