namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
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

internal sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 50;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger)
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

        // مهم: فقط پیام‌هایی که واقعاً Pending هستن و قدیمی‌تر از چند ثانیه نیستن (جلوگیری از race condition)
        var cutoff = DateTime.UtcNow.AddSeconds(-10);

        var messages = await db.OutboxMessages
            .Where(m => m.Status == "Pending" && m.CreatedAt <= cutoff)
            .OrderBy(m => m.CreatedAt)
            .Take(BatchSize)
            .ToListAsync(ct);

        if (!messages.Any())
            return;

        _logger.LogDebug("Processing {Count} outbox messages", messages.Count);

        foreach (var message in messages)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var @event = DeserializeEvent(message);
                if (@event is null)
                {
                    message.Status = "Failed";
                    message.Error = "Failed to deserialize event";
                    continue;
                }

                var dedupKey = $"outbox:{message.Id}";
                const int deduplicationTtlSeconds = 24 * 60 * 60; // 24 ساعت

                var isFirst = await dedup.TryMarkProcessedAsync(dedupKey, deduplicationTtlSeconds, ct);
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
                _logger.LogInformation("Outbox message {Id} processed successfully: {EventType}", message.Id,
                    @event.GetType().Name);
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
                    _logger.LogWarning("Outbox message {Id} moved to Dead state after {Retries} attempts", message.Id,
                        message.RetryCount);
                }
            }
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency issue while saving outbox changes. Will retry next cycle.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save outbox changes");
        }
    }

    private IDomainEvent? DeserializeEvent(OutboxMessage message)
    {
        try
        {
            var type = Type.GetType(message.Type, throwOnError: true);
            if (type is null) return null;

            return (IDomainEvent)JsonSerializer.Deserialize(message.Data, type,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize event type {Type} for message {Id}", message.Type, message.Id);
            return null;
        }
    }
}
