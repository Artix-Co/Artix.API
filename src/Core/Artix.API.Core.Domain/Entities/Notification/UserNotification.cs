namespace Artix.API.Core.Domain.Entities.Notification;

using Common;
using Enums;
using User;

public class UserNotification : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }

    public long NotificationId { get; private set; }
    public virtual Notification Notification { get; private set; }
    public bool IsRead { get; private set; } = false;
    public DateTime? ReadAt { get; private set; }
    public DeliveryStatus DeliveryStatus { get; private set; } = DeliveryStatus.Pending;
    public DateTime? DeliveredAt { get; private set; }

    // برای EF Core
    protected UserNotification()
    {
    }

    public UserNotification(long userId)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be valid.", nameof(userId));
        UserId = userId;
    }

    public void MarkAsDelivered()
    {
        if (DeliveryStatus != DeliveryStatus.Pending)
            return; // Idempotent
        DeliveryStatus = DeliveryStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        if (IsRead)
            return; // Idempotent
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        DeliveryStatus = DeliveryStatus.Failed;
    }
}
