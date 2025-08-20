namespace Artix.API.Core.DomainService.Interfaces.Notification;

using Contract.Features.Notifications.Commands.AddUserNotification;
using Infra.RabbitMQ.Models.Notification;

public interface INotificationServiceProvider
{
    Task SendUserNotificationAsync(AddUserNotificationCommand command, CancellationToken cancellationToken = default);

    Task SendBroadcastNotificationAsync(AddUserNotificationCommand command,
        CancellationToken cancellationToken = default);
}
