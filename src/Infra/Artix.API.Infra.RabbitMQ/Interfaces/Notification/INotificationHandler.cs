namespace Artix.API.Infra.RabbitMQ.Interfaces.Notification;

using Artix.API.Infra.RabbitMQ.Models.Notification;

public interface INotificationHandler
{
    Task HandleAsync(NotificationMessage message);
}
