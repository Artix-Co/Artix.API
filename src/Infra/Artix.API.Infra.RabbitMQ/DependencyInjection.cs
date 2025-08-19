namespace Artix.API.Infra.RabbitMQ;

using Interfaces.Notification;
using Interfaces.Outbox;
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
        services.AddHostedService<NotificationConsumerHostedService>();
        services.AddSingleton<INotificationService, NotificationService>();

        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        services.AddHostedService<OutboxProcessor>();
    }
}
