namespace Artix.API.Core.Contract.Primitives.DomainServices.Notification;

using Features.Notifications.Commands.AddUserNotification;

public interface INotificationServiceProvider
{
    Task SendUserNotificationAsync(AddUserNotificationCommand command, CancellationToken cancellationToken = default);

    Task SendBroadcastNotificationAsync(AddUserNotificationCommand command,
        CancellationToken cancellationToken = default);
}
