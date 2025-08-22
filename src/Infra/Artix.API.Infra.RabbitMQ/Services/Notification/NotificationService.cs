namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Interfaces.Notification;
using Models.Notification;

internal sealed class NotificationService : INotificationService
{
    private readonly INotificationProducer _notificationProducer;

    public NotificationService(INotificationProducer notificationProducer)
    {
        this._notificationProducer = notificationProducer;
    }

    public async Task SendUserNotificationAsync(NotificationMessage message,
        CancellationToken cancellationToken = default)
    {
        string routingKey = $"notifications.user.{message.UserId}";
        await this._notificationProducer.PublishAsync("notifications", routingKey, message, cancellationToken);
    }

    public async Task SendBroadcastNotificationAsync(NotificationMessage message,
        CancellationToken cancellationToken = default)
    {
        await this._notificationProducer.PublishAsync("notifications", "notifications.all", message, cancellationToken);
    }
}
