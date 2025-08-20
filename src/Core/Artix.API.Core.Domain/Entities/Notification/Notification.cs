namespace Artix.API.Core.Domain.Entities.Notification;

using Common;
using Enums;
using User;
using System;
using System.Collections.Generic;
using System.Linq;

public class Notification : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public string? Metadata { get; private set; }
    public long? SenderId { get; private set; }
    public bool IsBroadcast { get; private set; }
    public NotificationStatus Status { get; private set; } = NotificationStatus.Pending;
    public DateTime? SentAt { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public Priority Priority { get; private set; } = Priority.Medium;
    public int FailedAttempts { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<UserNotification> _userNotifications = new();
    public virtual IReadOnlyCollection<UserNotification> UserNotifications => _userNotifications.AsReadOnly();

    // برای EF Core
    protected Notification() { }

    // Factory برای ایجاد نوتیفیکیشن معمولی
    public static Notification CreateUserNotification(
        string title,
        string body,
        NotificationType type,
        long userId,
        long? senderId = null,
        string? metadata = null,
        DateTime? expirationDate = null,
        Priority priority = Priority.Medium)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be empty.", nameof(body));
        if (userId <= 0)
            throw new ArgumentException("UserId must be valid.", nameof(userId));

        var notification = new Notification
        {
            Title = title,
            Body = body,
            Type = type,
            Metadata = metadata,
            SenderId = senderId,
            IsBroadcast = false,
            ExpirationDate = expirationDate,
            Priority = priority,
            CreatedAt = DateTime.UtcNow,
            FailedAttempts = 0
        };

        notification.AddUser(userId);
        return notification;
    }

    // Factory برای نوتیفیکیشن broadcast
    public static Notification CreateBroadcastNotification(
        string title,
        string body,
        NotificationType type,
        long? senderId = null,
        string? metadata = null,
        DateTime? expirationDate = null,
        Priority priority = Priority.Medium)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be empty.", nameof(body));

        return new Notification
        {
            Title = title,
            Body = body,
            Type = type,
            Metadata = metadata,
            SenderId = senderId,
            IsBroadcast = true,
            ExpirationDate = expirationDate,
            Priority = priority,
            CreatedAt = DateTime.UtcNow,
            FailedAttempts = 0
        };
    }

    // متدهای دامنه (بدون تغییر، اما با FailedAttempts = 0 در MarkAsSent)
    public void AddUser(long userId)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be valid.", nameof(userId));
        if (IsBroadcast)
            throw new InvalidOperationException("Cannot add user to broadcast notification.");
        if (_userNotifications.Any(un => un.UserId == userId))
            return;

        _userNotifications.Add(new UserNotification(userId));
    }

    public void MarkAsSent()
    {
        if (Status != NotificationStatus.Pending)
            throw new InvalidOperationException("Only pending notifications can be marked as sent.");
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        FailedAttempts = 0;
        foreach (var userNotification in _userNotifications)
        {
            userNotification.MarkAsDelivered();
        }
    }

    public void MarkAsFailed(string errorMessage)
    {
        FailedAttempts++;
        LastErrorMessage = errorMessage;
        if (FailedAttempts >= 5)
            Status = NotificationStatus.Failed;
    }

    public bool IsExpired() => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;
}
