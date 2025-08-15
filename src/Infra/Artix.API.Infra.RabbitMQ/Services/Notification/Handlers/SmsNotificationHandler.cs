namespace Artix.API.Infra.RabbitMQ.Services.Notification.Handlers;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class SmsNotificationHandler : INotificationHandler
{
    private readonly ISmsService _smsService;

    public SmsNotificationHandler(ISmsService smsService)
    {
        this._smsService = smsService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return this._smsService.SendSmsAsync(message);
    }
}
