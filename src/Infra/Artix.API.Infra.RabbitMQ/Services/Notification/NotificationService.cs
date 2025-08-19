namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Interfaces.Notification;
using Models.Notification;

public class NotificationService : INotificationService
{
    private readonly INotificationProducer _notificationProducer;

    public NotificationService(INotificationProducer notificationProducer)
    {
        this._notificationProducer = notificationProducer;
    }

    public async Task SendUserNotificationAsync(NotificationMessage message)
    {
        // routing key برای کاربر خاص: notifications.user.{UserId}
        string routingKey = $"notifications.user.{message.UserId}";
        await this._notificationProducer.PublishAsync("notifications", routingKey, message);
    }

    public async Task SendBroadcastNotificationAsync(NotificationMessage message)
    {
        // routing key برای broadcast: notifications.all
        await this._notificationProducer.PublishAsync("notifications", "notifications.all", message);
    }
}
