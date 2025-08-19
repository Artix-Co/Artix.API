namespace Artix.API.Infra.RabbitMQ.Interfaces.Notification;

 

public interface INotificationProducer
{
    Task PublishAsync<T>(string exchange, string routingKey, T message,
        CancellationToken cancellationToken = default);
}
