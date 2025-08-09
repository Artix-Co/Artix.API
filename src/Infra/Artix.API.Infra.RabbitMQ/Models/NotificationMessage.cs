namespace Artix.API.Infra.RabbitMQ.Models;

public record NotificationMessage(
    Guid NotificationId,
    long UserId,
    string Title,
    string Body,
    NotificationType Type,
    DateTime CreatedAt,
    string? Metadata
);

public enum NotificationType
{
    InApp,
    Push,
    Email,
    Sms
}
