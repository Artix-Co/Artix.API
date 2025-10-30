namespace Artix.API.Core.Domain.Entities.Quiz;

using System;
using Common;
using Quest.Enums;
using User;

public class UserQuiz : BaseEntity
{
    public long UserId { get; private set; } // FK to AppUser.Id (SQL)
    public virtual AppUser User { get; private set; }

    public Guid QuestId { get; private set; } // لینک به Quest.Id در Mongo (Guid از AggregateRoot)

    // وضعیت per-user
    public QuizStatus Status { get; private set; } = QuizStatus.Pending;
    public DateTime? CompletedAt { get; private set; }
    public decimal Progress { get; private set; } = 0; // درصد تکمیل (e.g., 0.5 برای نیمه‌تمام)

    // اولویت نمایش per-user (ممکنه بر اساس user customize بشه)
    public int Priority { get; private set; } // اولویت نمایش به این کاربر خاص

    // Constructor
    protected UserQuiz()
    {
    }

    public UserQuiz(long userId, Guid questId, int priority)
    {
        this.UserId = userId;
        this.QuestId = questId;
        this.Priority = priority;
    }

    // متد برای به‌روزرسانی پیشرفت
    public void UpdateProgress(decimal newProgress)
    {
        if (newProgress < 0 || newProgress > 1) throw new ArgumentOutOfRangeException(nameof(newProgress));
        this.Progress = newProgress;
    }

    // متد برای تکمیل (و raise event برای اضافه کردن XP)
    public void Complete()
    {
        if (this.Status == QuizStatus.Completed) return;
        this.Status = QuizStatus.Completed;
        this.CompletedAt = DateTime.UtcNow;
        this.Progress = 1;
    }

    // متد برای شروع
    public void Start()
    {
        if (this.Status == QuizStatus.Pending)
            this.Status = QuizStatus.InProgress;
    }
}
