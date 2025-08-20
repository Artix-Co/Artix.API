namespace Artix.API.Infra.RabbitMQ.Models.Notification;

using Core.Domain.Entities.Notification;
using Core.Domain.Entities.Notification.Enums;

public record NotificationMessage(
    Guid NotificationId,
    long? UserId,
    string Title,
    string Body,
    NotificationType Type,
    DateTime CreatedAt,
    string? Metadata
);

