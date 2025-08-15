namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class SmsService:ISmsService
{
    public Task SendSmsAsync(NotificationMessage message)
    {
        throw new NotImplementedException();
    }
}
