namespace Artix.API.Infra.RabbitMQ.Services;

using Interfaces;
using Models;

public class SmsService:ISmsService
{
    public Task SendSmsAsync(NotificationMessage message)
    {
        throw new NotImplementedException();
    }
}
