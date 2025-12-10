namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using Core.Domain.DomainEvents;
using MediatR;

public sealed class DomainEventNotification : INotification
{
    public IDomainEvent DomainEvent { get; }

    public DomainEventNotification(IDomainEvent domainEvent)
    {
        this.DomainEvent = domainEvent;
    }
}
