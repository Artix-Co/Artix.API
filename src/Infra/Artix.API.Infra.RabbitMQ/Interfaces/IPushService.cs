namespace Artix.API.Infra.RabbitMQ.Interfaces;

using Models;

public interface IPushService
{
    Task SendPushAsync(NotificationMessage message);
}
