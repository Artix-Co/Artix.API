namespace Artix.API.Infra.Sql.Data.Interceptors;

using Core.Domain.Entities.Common;
using Core.Domain.Persistence;
using Core.Domain.Persistence.Enums;
using DbContexts;
using Newtonsoft.Json;

internal sealed class DomainEventInterceptor : IChangeInterceptor
{
    public void BeforeSaveChanges(ArtixCommandDbContext context)
    {
        var aggregates = context.ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var @event in aggregate.DomainEvents)
            {
                var outboxMessage = new OutboxMessage
                {
                    Type = @event.GetType().AssemblyQualifiedName!,
                    Data = JsonConvert.SerializeObject(@event,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented
                        }),
                    Status = OutboxMessageStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                context.OutboxMessages.Add(outboxMessage);
            }

            aggregate.ClearDomainEvents();
        }
    }

    public async Task BeforeSaveChangesAsync(ArtixCommandDbContext context, CancellationToken cancellationToken)
    {
        BeforeSaveChanges(context); // منطق ناهمزمان اضافی ندارد
        await Task.CompletedTask;
    }
}
