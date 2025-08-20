namespace Artix.API.Core.Contract.Features.Notifications.Commands.AddUserNotification;

using Domain.Entities.Notification.Enums;
using Primitives.Handlers;

public sealed class AddUserNotificationCommand : ICommand
{
    public long UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public NotificationType Type { get; set; }
    public string? Metadata { get; set; }
}
