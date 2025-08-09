namespace Artix.API.Infra.RabbitMQ.Interfaces;

public interface INotificationProducer
{
    Task PublishAsync(Models.NotificationMessage message, string routingKey);
}
