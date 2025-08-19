namespace Artix.API.Infra.RabbitMQ.Interfaces.Notification;

using Models.Notification;

public interface INotificationService
{
    Task SendUserNotificationAsync(NotificationMessage message);
    Task SendBroadcastNotificationAsync(NotificationMessage message);
}
