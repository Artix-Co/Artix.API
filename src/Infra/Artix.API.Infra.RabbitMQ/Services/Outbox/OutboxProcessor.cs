namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
using Core.Contract.Primitives.Infra.RabbitMQ;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.DomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sql.Data.DbContexts;


internal sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var dedup = scope.ServiceProvider.GetRequiredService<IEventDeduplicationStore>();

                var messages = await context.OutboxMessages
                    .Where(m => m.Status == "Pending")
                    .Take(50)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        var eventType = Type.GetType(message.Type);
                        if (eventType == null)
                        {
                            message.Status = "Failed";
                            continue;
                        }

                        var @event = (IDomainEvent)JsonSerializer.Deserialize(message.Data, eventType, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })!;

                        var dedupKey = message.Id.ToString();
                        var isFirst = await dedup.TryMarkProcessedAsync(dedupKey, 3600, stoppingToken);
                        if (!isFirst)
                        {
                            message.Status = "Processed";
                            message.ProcessedAt = DateTime.UtcNow;
                            continue;
                        }

                        await publisher.PublishAsync(@event, stoppingToken);

                        var notification = new DomainEventNotification(@event);
                        await mediator.Publish(notification, stoppingToken);

                        message.Status = "Processed";
                        message.ProcessedAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process message {MessageId}", message.Id);
                        message.Status = "Failed";
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OutboxProcessor");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
