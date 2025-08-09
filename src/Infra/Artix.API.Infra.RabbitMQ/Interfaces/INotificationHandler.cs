namespace Artix.API.Infra.RabbitMQ.Interfaces;

using Models;

public interface INotificationHandler
{
    Task HandleAsync(NotificationMessage message);
}
