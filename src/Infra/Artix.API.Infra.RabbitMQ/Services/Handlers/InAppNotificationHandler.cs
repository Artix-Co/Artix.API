namespace Artix.API.Infra.RabbitMQ.Services.Handlers;

using Interfaces;
using Models;

public class InAppNotificationHandler : INotificationHandler
{
    private readonly IInAppService _inAppService;

    public InAppNotificationHandler(IInAppService inAppService)
    {
        _inAppService = inAppService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return _inAppService.CreateInAppNotificationAsync(message);
    }
}
