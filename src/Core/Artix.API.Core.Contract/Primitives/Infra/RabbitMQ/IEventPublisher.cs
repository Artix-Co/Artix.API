namespace Artix.API.Core.Contract.Primitives.Infra.RabbitMQ;

using Domain.DomainEvents;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
}
