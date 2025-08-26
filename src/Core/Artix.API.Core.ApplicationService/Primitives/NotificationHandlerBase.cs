namespace Artix.API.Core.ApplicationService.Primitives;

using Domain.DomainEvents;
using Infra.RabbitMQ.Services.Outbox;
using MediatR;

public abstract class NotificationHandlerBase<TEvent> : INotificationHandler<DomainEventNotification>
    where TEvent : IDomainEvent
{
    protected NotificationHandlerBase()
    {
    }


    public async Task Handle(DomainEventNotification notification, CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is TEvent domainEvent)
        {
            await HandleEventAsync(domainEvent, cancellationToken);
        }
    }

    protected abstract Task HandleEventAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
