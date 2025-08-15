namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class InAppService : IInAppService
{
    public Task CreateInAppNotificationAsync(NotificationMessage message)
    {
        throw new NotImplementedException();
    }
}
