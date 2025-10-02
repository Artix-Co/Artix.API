namespace Artix.API.Core.Domain.Entities.Quest;

using System;
using Common;
using Enums;
using User;

public class UserQuest : BaseEntity
{
    public long UserId { get; private set; } // FK to AppUser.Id (SQL)
    public virtual AppUser User { get; private set; }

    public Guid QuestId { get; private set; } // لینک به Quest.Id در Mongo (Guid از AggregateRoot)

    // وضعیت per-user
    public QuestStatus Status { get; private set; } = QuestStatus.Pending;
    public DateTime? CompletedAt { get; private set; }
    public decimal Progress { get; private set; } = 0; // درصد تکمیل (e.g., 0.5 برای نیمه‌تمام)

    // اولویت نمایش per-user (ممکنه بر اساس user customize بشه)
    public int Priority { get; private set; } // اولویت نمایش به این کاربر خاص

    // Constructor
    protected UserQuest()
    {
    }

    public UserQuest(long userId, Guid questId, int priority)
    {
        UserId = userId;
        QuestId = questId;
        Priority = priority;
    }

    // متد برای به‌روزرسانی پیشرفت
    public void UpdateProgress(decimal newProgress)
    {
        if (newProgress < 0 || newProgress > 1) throw new ArgumentOutOfRangeException(nameof(newProgress));
        Progress = newProgress;
    }

    // متد برای تکمیل (و raise event برای اضافه کردن XP)
    public void Complete()
    {
        if (Status == QuestStatus.Completed) return;
        Status = QuestStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Progress = 1;
    }

    // متد برای شروع
    public void Start()
    {
        if (Status == QuestStatus.Pending)
            Status = QuestStatus.InProgress;
    }
}
