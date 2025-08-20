namespace Artix.API.Core.Domain.Entities.Notification.Enums;

public enum NotificationStatus
{
    Pending,  // منتظر ارسال
    Sent,     // ارسال شده
    Failed,   // شکست خورده
    Expired   // منقضی شده
}
