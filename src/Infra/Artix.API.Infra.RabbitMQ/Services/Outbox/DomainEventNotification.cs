namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using Artix.API.Core.Domain.DomainEvents;
using MediatR;

public class DomainEventNotification : INotification
{
    public IDomainEvent DomainEvent { get; }

    public DomainEventNotification(IDomainEvent domainEvent)
    {
        this.DomainEvent = domainEvent;
    }
}
