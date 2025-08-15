namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class PushService: IPushService
{
    public Task SendPushAsync(NotificationMessage message)
    {
        throw new NotImplementedException();
    }
}
