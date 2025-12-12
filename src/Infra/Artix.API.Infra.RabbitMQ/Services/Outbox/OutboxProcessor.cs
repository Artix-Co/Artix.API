namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using Core.Contract.Primitives.Infra.RabbitMQ;
using Core.Contract.Primitives.Infra.Redis;
using Core.Domain.DomainEvents;
using Core.Domain.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sql.Data.DbContexts;
using StackExchange.Redis;
using Utils;

internal sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 50;


    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                _logger.LogError(ex, "Critical error in OutboxProcessor loop");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("OutboxProcessor stopped.");
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dedup = scope.ServiceProvider.GetRequiredService<IEventDeduplicationStore>();

        var cutoff = DateTime.UtcNow.AddSeconds(-10);

        var messages = await db.OutboxMessages
            .Where(m => m.Status == "Pending" && m.CreatedAt <= cutoff)
            .OrderBy(m => m.CreatedAt)
            .Take(BatchSize)
            .ToListAsync(ct);

        if (!messages.Any()) return;

        _logger.LogDebug("Processing {Count} outbox messages", messages.Count);

        foreach (var message in messages)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var @event = DeserializeEvent(message.Data, message.Type);
                if (@event is null)
                {
                    message.Status = "Failed";
                    message.Error = "Failed to deserialize event";
                    continue;
                }

                var dedupKey = $"outbox:{message.Id}";
                const int deduplicationTtlSeconds = 24 * 60 * 60;

                bool isFirst = true;
                try
                {
                    isFirst = await dedup.TryMarkProcessedAsync(dedupKey, deduplicationTtlSeconds, ct);
                }
                catch (Exception ex) when (ex is RedisConnectionException ||
                                           ex.InnerException is RedisConnectionException)
                {
                    _logger.LogWarning(ex,
                        "Redis unavailable for deduplication. Proceeding without dedup. MessageId: {Id}", message.Id);
                }

                if (!isFirst)
                {
                    message.Status = "Processed";
                    message.ProcessedAt = DateTime.UtcNow;
                    _logger.LogDebug("Skipped duplicate outbox message {Id}", message.Id);
                    continue;
                }

                await publisher.PublishAsync(@event, ct);

                var notification = new DomainEventNotification(@event);
                await mediator.Publish(notification, ct);

                message.Status = "Processed";
                message.ProcessedAt = DateTime.UtcNow;
                _logger.LogInformation("Outbox message {Id} processed: {EventType}", message.Id, @event.GetType().Name);
            }
            catch (Exception ex)
            {
                message.Status = "Failed";
                message.Error = ex.Message;
                message.RetryCount++;

                _logger.LogError(ex, "Failed to process outbox message {Id} (Attempt {Retry})", message.Id,
                    message.RetryCount);

                if (message.RetryCount >= 5)
                {
                    message.Status = "Dead";
                    _logger.LogWarning("Outbox message {Id} moved to Dead state", message.Id);
                }
            }
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save outbox changes");
        }
    }


    private IDomainEvent? DeserializeEvent(string jsonData, string typeFullName)
    {
        try
        {
            var type = Type.GetType(typeFullName, throwOnError: true)!;
            if (!typeof(IDomainEvent).IsAssignableFrom(type))
                throw new InvalidOperationException($"Type {type} is not IDomainEvent");

            return jsonData.FromOutboxJson(type) as IDomainEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize event from Outbox. Type: {Type}", typeFullName);
            return null;
        }
    }
}
