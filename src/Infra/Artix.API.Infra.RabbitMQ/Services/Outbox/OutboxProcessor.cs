namespace Artix.API.Infra.RabbitMQ.Services.Outbox;

using System.Text.Json;
using Core.Domain.DomainEvents;
using Interfaces.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sql.Data.DbContexts;

public class OutboxProcessor : BackgroundService
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
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                    var messages = await context.OutboxMessages
                        .Where(m => m.Status == "Pending")
                        .Take(50)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        try
                        {
                            var eventType = GetEventType(message.Type);
                            if (eventType == null)
                            {
                                _logger.LogWarning("Event type {Type} not found.", message.Type);
                                message.Status = "Failed";
                                continue;
                            }

                            var @event = (IDomainEvent)JsonSerializer.Deserialize(message.Data, eventType);
                            await publisher.PublishAsync(@event, stoppingToken);
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
        // تلاش برای پیدا کردن نوع در Assembly فعلی
        var type = Type.GetType(typeName);
        if (type != null)
            return type;

        // جستجو در Assembly حاوی IDomainEvent
        var domainAssembly = typeof(IDomainEvent).Assembly;
        type = domainAssembly.GetType(typeName);
        if (type != null)
            return type;

        // جستجو در تمام Assemblyهای لودشده
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        // لاگ برای دیباگ
        _logger.LogInformation("Loaded assemblies: {Assemblies}",
            string.Join(", ", AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name)));
        return null;
    }
}
