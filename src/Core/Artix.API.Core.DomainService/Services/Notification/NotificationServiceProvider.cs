namespace Artix.API.Core.DomainService.Services.Notification;

using Contract.Features.Notifications.Commands;
using Contract.Features.Notifications.Commands.AddUserNotification;
using Domain.Entities.Notification;
using Infra.RabbitMQ.Interfaces.Notification;
using Infra.RabbitMQ.Models.Notification;
using Interfaces.Notification;
using System.Threading.Tasks;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;

public class NotificationServiceProvider : INotificationServiceProvider
{
    private readonly INotificationService _notificationService;
    private readonly INotificationCommandRepository _notificationCommandRepository;
    private readonly UserManager<AppUser> _userManager;

    public NotificationServiceProvider(
        INotificationService notificationService, INotificationCommandRepository notificationCommandRepository,
        UserManager<AppUser> userManager)
    {
        _notificationService = notificationService;
        _notificationCommandRepository = notificationCommandRepository;
        _userManager = userManager;
    }

    public async Task SendUserNotificationAsync(AddUserNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        var notification = Notification.CreateUserNotification(
            title: command.Title,
            body: command.Body,
            type: command.Type,
            userId: command.UserId,
            metadata: command.Metadata
        );

        await _notificationCommandRepository.InsertAsync(notification, cancellationToken);

        var notificationMessage = new NotificationMessage(
            NotificationId: notification.BusinessId,
            UserId: command.UserId,
            Title: notification.Title,
            Body: notification.Body,
            Type: notification.Type,
            CreatedAt: notification.CreatedAt,
            Metadata: notification.Metadata
        );

        await _notificationService.SendUserNotificationAsync(notificationMessage, cancellationToken);
    }

    public async Task SendBroadcastNotificationAsync(AddUserNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        var notification = Notification.CreateBroadcastNotification(
            title: command.Title,
            body: command.Body,
            type: command.Type,
            metadata: command.Metadata
        );

        // TODO: need to review this codee to prevent N+1 problems
        var userIds = _userManager.Users.Select(u => u.Id).ToList();
        foreach (var userId in userIds)
        {
            notification.AddUser(userId);
        }

        await _notificationCommandRepository.InsertAsync(notification, cancellationToken);

        var notificationMessage = new NotificationMessage(
            NotificationId: notification.BusinessId,
            UserId: null,
            Title: notification.Title,
            Body: notification.Body,
            Type: notification.Type,
            CreatedAt: notification.CreatedAt,
            Metadata: notification.Metadata
        );

        await _notificationService.SendBroadcastNotificationAsync(notificationMessage, cancellationToken);
    }
}
