namespace Artix.API.Infra.RabbitMQ.Interfaces;

using Models;

public interface ISmsService
{
    Task SendSmsAsync(NotificationMessage message);
}
