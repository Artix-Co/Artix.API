namespace Artix.API.Core.ApplicationService.EventHandlers.Object;

using Primitives;
using Artix.API.Core.Contract.Features.Notifications.Commands.AddUserNotification;
using Domain.Entities.Notification.Enums;
using Artix.API.Core.Domain.Entities.Object.Events;
using DomainService.Interfaces.Notification;

internal sealed class RepeatUserScanEventHandler : NotificationHandlerBase<RepeatUserScanEvent>
{
    private readonly INotificationServiceProvider _notificationServiceProvider;

    public RepeatUserScanEventHandler(INotificationServiceProvider notificationServiceProvider)
    {
        this._notificationServiceProvider = notificationServiceProvider;
    }
    protected override async Task HandleEventAsync(RepeatUserScanEvent domainEvent,
        CancellationToken cancellationToken)
    {
        // TODO: calculate user xp
        var userNotification = new AddUserNotificationCommand
        (
            domainEvent.UserId,
            "notification from service provider",
            "you scanned an obj",
            NotificationType.InApp,
            null
        );
       
        await this._notificationServiceProvider.SendUserNotificationAsync(userNotification, cancellationToken);

        
    }
}
