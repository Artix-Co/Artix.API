namespace Artix.API.Infra.RabbitMQ.Interfaces.Notification;

using Models.Notification;

public interface INotificationService
{
    Task SendUserNotificationAsync(NotificationMessage message, CancellationToken cancellationToken = default);
    Task SendBroadcastNotificationAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
