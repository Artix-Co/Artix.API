namespace Artix.API.Infra.RabbitMQ;

using Interfaces.Notification;
using Interfaces.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Notification;
using Services.Notification.Handlers;
using Services.Outbox;

public static class DependencyInjection
{
    public static void AddRabbitMqService(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqConnectionFactory>();
        services.AddSingleton<IMessageSerializer, MessageSerializer>();
        services.AddSingleton<INotificationProducer, NotificationProducer>();

        services.AddTransient<IInAppService, InAppService>(); 
        services.AddTransient<IPushService, PushService>(); 
        services.AddTransient<IEmailService, EmailService>(); 
        services.AddTransient<ISmsService, SmsService>(); 

        services.AddTransient<INotificationHandler, InAppNotificationHandler>();
        services.AddTransient<INotificationHandler, PushNotificationHandler>();
        services.AddTransient<INotificationHandler, EmailNotificationHandler>();
        services.AddTransient<INotificationHandler, SmsNotificationHandler>();

        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();
        
    
        services.AddHostedService<NotificationConsumer>();

        services.AddHostedService<OutboxProcessor>();
    }
}
