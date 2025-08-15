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

        // Register the missing service interfaces with their implementations
        services.AddTransient<IInAppService, InAppService>(); // Replace InAppService with your actual implementation
        services.AddTransient<IPushService, PushService>(); // Replace PushService with your actual implementation
        services.AddTransient<IEmailService, EmailService>(); // Replace EmailService with your actual implementation
        services.AddTransient<ISmsService, SmsService>(); // Replace SmsService with your actual implementation

        // Register all notification handlers
        services.AddTransient<INotificationHandler, InAppNotificationHandler>();
        services.AddTransient<INotificationHandler, PushNotificationHandler>();
        services.AddTransient<INotificationHandler, EmailNotificationHandler>();
        services.AddTransient<INotificationHandler, SmsNotificationHandler>();

        // ثبت IEventPublisher برای OutboxProcessor
        services.AddScoped<IEventPublisher, RabbitMQEventPublisher>();
        
        // Register NotificationConsumer as a hosted service
        services.AddHostedService<NotificationConsumer>(sp =>
        {
            var factory = sp.GetRequiredService<RabbitMqConnectionFactory>();
            var serializer = sp.GetRequiredService<IMessageSerializer>();
            var handler = sp.GetRequiredService<INotificationHandler>();

            var queueName = "notifications.queue"; // مقدار مورد نظر خودت اینجا بذار

            return new NotificationConsumer(factory, serializer, handler, queueName);
        });

        services.AddHostedService<OutboxProcessor>();
    }
}
