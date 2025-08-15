namespace Artix.API.Infra.RabbitMQ.Interfaces.Outbox;

using Core.Domain.DomainEvents;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
}
