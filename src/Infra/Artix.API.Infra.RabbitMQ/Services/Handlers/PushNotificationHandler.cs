namespace Artix.API.Infra.RabbitMQ.Services.Handlers;

using Interfaces;
using Models;

public class PushNotificationHandler : INotificationHandler
{
    private readonly IPushService _pushService;

    public PushNotificationHandler(IPushService pushService)
    {
        _pushService = pushService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return _pushService.SendPushAsync(message);
    }
}
