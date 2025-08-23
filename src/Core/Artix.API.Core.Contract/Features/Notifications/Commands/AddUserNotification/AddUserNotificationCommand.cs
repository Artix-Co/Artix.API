namespace Artix.API.Core.Contract.Features.Notifications.Commands.AddUserNotification;

using Domain.Entities.Notification.Enums;
using Primitives.Handlers;

public sealed record AddUserNotificationCommand(
    long UserId,
    string Title,
    string Body,
    NotificationType Type,
    string? Metadata) : ICommand;
