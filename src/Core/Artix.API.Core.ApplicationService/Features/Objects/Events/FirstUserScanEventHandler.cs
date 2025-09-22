namespace Artix.API.Core.ApplicationService.Features.Objects.Events;

using Contract.Features.Notifications.Commands.AddUserNotification;
using Domain.Entities.Notification.Enums;
using Domain.Entities.Object.Events;
using DomainService.Interfaces.Notification;
using DomainService.Interfaces.XPRules;
using Primitives;

internal sealed class FirstUserScanEventHandler : NotificationHandlerBase<FirstUserScanEvent>
{
    private readonly INotificationServiceProvider _notificationServiceProvider;
    private readonly IXpRulesService _xpRulesService;

    public FirstUserScanEventHandler(INotificationServiceProvider notificationServiceProvider,
        IXpRulesService xpRulesService)
    {
        this._notificationServiceProvider = notificationServiceProvider;
        this._xpRulesService = xpRulesService;
    }

    protected override async Task HandleEventAsync(FirstUserScanEvent domainEvent,
        CancellationToken cancellationToken)
    {
        await this._xpRulesService.CalculateXpForFirstScanAsync(domainEvent.UserId, domainEvent.BusinessId,
            cancellationToken: cancellationToken);
        var userNotification = new AddUserNotificationCommand
        (
            domainEvent.UserId,
            "notification from service provider",
            "you scanned an obj for the first time!",
            NotificationType.InApp,
            null
        );

        await this._notificationServiceProvider.SendUserNotificationAsync(userNotification, cancellationToken);
    }
}
