namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
using Core.Domain.DomainEvents;
using Interfaces.Outbox;
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
                await using var scope = this._scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                
                var messages = await context.OutboxMessages
                    .Where(m => m.Status == "Pending")
                    .Take(50)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        var eventType = this.GetEventType(message.Type);
                        if (eventType == null)
                        {
                            this._logger.LogWarning("Event type {Type} not found.", message.Type);
                            message.Status = "Failed";
                            continue;
                        }

                        var @event = (IDomainEvent)JsonSerializer.Deserialize(message.Data, eventType)!;
                        await publisher.PublishAsync(@event, stoppingToken);
                        
                         

                        // انتشار به MediatR
                        var notification = new DomainEventNotification(@event);
                        await mediator.Publish(notification, stoppingToken);
                        
                        
                        message.Status = "Processed";
                        message.ProcessedAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Failed to process message {MessageId}", message.Id);
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

    private Type? GetEventType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null)
            return type;

        var domainAssembly = typeof(IDomainEvent).Assembly;
        type = domainAssembly.GetType(typeName);
        if (type != null)
            return type;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        _logger.LogInformation("Loaded assemblies: {Assemblies}", string.Join(", ", AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name)));
        return null;
    }
}
