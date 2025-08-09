namespace Artix.API.Infra.RabbitMQ.Interfaces;

using Models;

public interface IInAppService
{
    Task CreateInAppNotificationAsync(NotificationMessage message);
}
