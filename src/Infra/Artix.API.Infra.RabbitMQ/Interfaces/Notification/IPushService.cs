namespace Artix.API.Infra.RabbitMQ.Interfaces.Notification;

using Artix.API.Infra.RabbitMQ.Models.Notification;

public interface IPushService
{
    Task SendPushAsync(NotificationMessage message);
}
