namespace Artix.API.Core.Contract.Primitives.Infra.RabbitMQ;

public interface INotificationService
{
    Task SendUserNotificationAsync(NotificationMessage message, CancellationToken cancellationToken = default);
    Task SendBroadcastNotificationAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
