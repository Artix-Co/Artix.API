namespace Artix.API.Core.ApplicationService.Primitives;

using Infra.RabbitMQ.Services.Outbox;
using MediatR;

public abstract class DomainEventNotificationHandlerBase : INotificationHandler<DomainEventNotification>
{
    public Task Handle(DomainEventNotification notification, CancellationToken cancellationToken)
    {
       
        Console.WriteLine("Hello World!");

        return Task.CompletedTask;
    }
}
