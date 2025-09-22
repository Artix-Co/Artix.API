namespace Artix.API.Core.ApplicationService.EventHandlers.Object;

using Primitives;
using Artix.API.Core.Contract.Features.Notifications.Commands.AddUserNotification;
using Domain.Entities.Notification.Enums;
using Artix.API.Core.Domain.Entities.Object.Events;
using DomainService.Interfaces.Notification;
using DomainService.Interfaces.TierCalculator;
using DomainService.Interfaces.XPRules;

internal sealed class RepeatUserScanEventHandler : NotificationHandlerBase<RepeatUserScanEvent>
{
    private readonly INotificationServiceProvider _notificationServiceProvider;
    private readonly IXpRulesService _xpRulesService;


    public RepeatUserScanEventHandler(INotificationServiceProvider notificationServiceProvider,
        IXpRulesService xpRulesService, ITierCalculatorService tierCalculatorService)
    {
        this._notificationServiceProvider = notificationServiceProvider;
        this._xpRulesService = xpRulesService;
    }

    protected override async Task HandleEventAsync(RepeatUserScanEvent domainEvent,
        CancellationToken cancellationToken)
    {
        await this._xpRulesService.CalculateXpForRepeatScanAsync(domainEvent.UserId, domainEvent.ObjectBusinessId,
            cancellationToken: cancellationToken);

        var userNotification = new AddUserNotificationCommand
        (
            domainEvent.UserId,
            "notification from service provider",
            "you scanned an obj is repeated!",
            NotificationType.InApp,
            null
        );

        await this._notificationServiceProvider.SendUserNotificationAsync(userNotification, cancellationToken);
    }
}
