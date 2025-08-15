namespace Artix.API.Infra.RabbitMQ.Interfaces.Notification;

using Artix.API.Infra.RabbitMQ.Models.Notification;

public interface INotificationProducer
{
    Task PublishAsync(NotificationMessage message, string routingKey);
}
