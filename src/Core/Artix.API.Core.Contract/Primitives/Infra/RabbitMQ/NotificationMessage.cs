namespace Artix.API.Core.Contract.Primitives.Infra.RabbitMQ;

using Domain.Entities.Notification.Enums;

public record NotificationMessage(
    Guid NotificationId,
    long? UserId,
    string Title,
    string Body,
    NotificationType Type,
    DateTime CreatedAt,
    string? Metadata
);

