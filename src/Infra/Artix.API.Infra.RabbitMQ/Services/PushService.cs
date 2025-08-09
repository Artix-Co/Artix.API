namespace Artix.API.Infra.RabbitMQ.Services;

using Interfaces;
using Models;

public class PushService: IPushService
{
    public Task SendPushAsync(NotificationMessage message)
    {
        throw new NotImplementedException();
    }
}
