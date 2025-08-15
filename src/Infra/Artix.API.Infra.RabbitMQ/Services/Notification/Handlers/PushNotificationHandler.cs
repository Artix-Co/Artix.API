namespace Artix.API.Infra.RabbitMQ.Services.Notification.Handlers;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class PushNotificationHandler : INotificationHandler
{
    private readonly IPushService _pushService;

    public PushNotificationHandler(IPushService pushService)
    {
        this._pushService = pushService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return this._pushService.SendPushAsync(message);
    }
}
