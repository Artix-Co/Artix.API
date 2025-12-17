namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Events;

using Contract.Features.Notifications.Commands.AddUserNotification;
using Contract.Primitives.DomainServices.Notification;
using Contract.Primitives.DomainServices.TierCalculator;
using Contract.Primitives.DomainServices.XPRules;
using Domain.Entities.Notification.Enums;
using Domain.Entities.Object.Events;
using Primitives;

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
