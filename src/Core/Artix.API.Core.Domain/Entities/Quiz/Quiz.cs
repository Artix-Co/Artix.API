namespace Artix.API.Core.Domain.Entities.Quiz;

using System;
using System.Collections.Generic;
using Common;
using Quest.Enums;
using Events;
using ValueObjects;

public class Quiz : AggregateRoot
{
    // فیلدهای اصلی (عمومی برای همه کاربرا)
    public string Title { get; private set; } // عنوان quest، مثل "اسکن 5 QR در موزه تهران"
    public string Description { get; private set; } // توضیحات، مثل "برای کشف اشیاء خاص و کسب XP"

    // اقدامات مورد نیاز (dynamic list برای اقدامات مختلف مثل اسکن QR، تکمیل کوئیز، یا فعالیت Strike)
    public List<QuizAction> RequiredActions { get; private set; } = new List<QuizAction>();

    // پاداش‌ها (فقط پایه – XP نهایی در UserXp محاسبه می‌شه)
    public int XPReward { get; private set; } // XP پایه که با تکمیل quest داده می‌شه
    public int BonusXP { get; private set; } // XP اضافی برای golden level یا ایونت‌های فصلی

    // Level و اولویت
    public int Tier { get; private set; } // level/tier quest (مثل 1 برای ساده، 3 برای پیشرفته)
    public int Priority { get; private set; } // اولویت نمایش به کاربر (بالاتر = اول نمایش بده، e.g., 10 برای فصلی)

    // وضعیت عمومی quest (نه per-user)
    public QuizStatus Status { get; private set; } =
        QuizStatus.Pending; // enum برای وضعیت کلی (e.g., Active/Inactive)

    public DateTime? Deadline { get; private set; } // مهلت تکمیل (برای questهای فصلی)

    // لینک به اشیاء (از SQL)
    public List<long> RelatedObjectIds { get; private set; } = new(); // لینک به Object.Id در SQL (اشیاء موزه/QR Codes)

    // اتصال به فیچرهای Artix
    public bool IsSeasonal { get; private set; } // آیا بخشی از ایونت فصلی مثل نوروز هست؟
    public string RelatedFeature { get; private set; } // مثل "QRHunts", "CoUp", "Strike", "LastQuiz"

    // Constructor برای ایجاد quest جدید (بدون UserId – عمومی)
    protected Quiz()
    {
    } // برای serialization

    public Quiz(string title, string description, int xpReward, int bonusXp, int tier, int priority, DateTime? deadline = null, bool isSeasonal = false, string relatedFeature = null)
    {
        this.Title = title;
        this.Description = description;
        this.XPReward = xpReward;
        this.BonusXP = bonusXp;
        this.Tier = tier;
        this.Priority = priority;
        this.Deadline = deadline;
        this.IsSeasonal = isSeasonal;
        this.RelatedFeature = relatedFeature;
        this.RaiseDomainEvent(new QuizCreatedEvent(this.BusinessId));
    }

    public void AddAction(string actionType, string details, int requiredCount)
    {
        this.RequiredActions.Add(new QuizAction(actionType, details, requiredCount));
    }

 
}
