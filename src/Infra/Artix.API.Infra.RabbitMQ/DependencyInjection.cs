namespace Artix.API.Infra.RabbitMQ;

using Core.Contract.Primitives.Infra.RabbitMQ;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;
using Services.Notification;
using Services.Outbox;

public static class DependencyInjection
{
    public static void AddRabbitMqService(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqConnectionFactory>();

        services.AddSingleton<IConnection>(sp =>
        {
            var factory = sp.GetRequiredService<RabbitMqConnectionFactory>();
            var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();


            connection.ConnectionShutdownAsync += (_, args) =>
            {
                var logger = sp.GetRequiredService<ILogger<IConnection>>();
                logger.LogCritical("RabbitMQ connection lost. Reason: {Reason}", args.ReplyText);
                return Task.CompletedTask;
            };

            connection.RecoverySucceededAsync += (_, _) =>
            {
                var logger = sp.GetRequiredService<ILogger<IConnection>>();
                logger.LogInformation("RabbitMQ connection recovered successfully.");
                return Task.CompletedTask;
            };

            return connection;
        });

        // Producerها و سرویس‌ها به صورت Singleton
        services.AddSingleton<INotificationProducer, NotificationProducer>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        // BackgroundServiceها
        services.AddHostedService<NotificationOutboxProcessor>();
        services.AddHostedService<NotificationConsumerHostedService>();
        services.AddHostedService<OutboxProcessor>();
    }
}
