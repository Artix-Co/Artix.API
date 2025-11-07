namespace Artix.API.Core.DomainService.Interfaces.Notification;

using Contract.Features.Notifications.Commands.AddUserNotification;

public interface INotificationServiceProvider
{
    Task SendUserNotificationAsync(AddUserNotificationCommand command, CancellationToken cancellationToken = default);

    Task SendBroadcastNotificationAsync(AddUserNotificationCommand command,
        CancellationToken cancellationToken = default);
}
