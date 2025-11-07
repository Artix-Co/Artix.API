namespace Artix.API.Infra.RabbitMQ;

using Core.Contract.Primitives.Infra.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Notification;
using Services.Outbox;

public static class DependencyInjection
{
    public static void AddRabbitMqService(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqConnectionFactory>();
        services.AddSingleton<INotificationProducer, NotificationProducer>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        services.AddHostedService<NotificationOutboxProcessor>();
        services.AddHostedService<NotificationConsumerHostedService>();
        services.AddHostedService<OutboxProcessor>();
    }
}
