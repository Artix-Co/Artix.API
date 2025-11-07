namespace Artix.API.Core.Contract.Primitives.Infra.RabbitMQ;

public interface INotificationProducer
{
    Task PublishAsync<T>(string exchange, string routingKey, T message,
        CancellationToken cancellationToken = default);
}
