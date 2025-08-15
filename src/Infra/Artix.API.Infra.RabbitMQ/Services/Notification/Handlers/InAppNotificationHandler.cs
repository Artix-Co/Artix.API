namespace Artix.API.Infra.RabbitMQ.Services.Notification.Handlers;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class InAppNotificationHandler : INotificationHandler
{
    private readonly IInAppService _inAppService;

    public InAppNotificationHandler(IInAppService inAppService)
    {
        this._inAppService = inAppService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return this._inAppService.CreateInAppNotificationAsync(message);
    }
}
