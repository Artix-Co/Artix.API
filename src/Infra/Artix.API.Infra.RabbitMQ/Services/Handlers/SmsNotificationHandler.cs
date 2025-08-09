namespace Artix.API.Infra.RabbitMQ.Services.Handlers;

using Interfaces;
using Models;

public class SmsNotificationHandler : INotificationHandler
{
    private readonly ISmsService _smsService;

    public SmsNotificationHandler(ISmsService smsService)
    {
        _smsService = smsService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return _smsService.SendSmsAsync(message);
    }
}
