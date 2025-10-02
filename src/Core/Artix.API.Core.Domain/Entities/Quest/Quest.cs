namespace Artix.API.Core.Domain.Entities.Quest;

using System;
using System.Collections.Generic;
using Common;
using Enums;
using Events;
using ValueObjects;

public class Quest : AggregateRoot
{
    // فیلدهای اصلی (عمومی برای همه کاربرا)
    public string Title { get; private set; } // عنوان quest، مثل "اسکن 5 QR در موزه تهران"
    public string Description { get; private set; } // توضیحات، مثل "برای کشف اشیاء خاص و کسب XP"

    // اقدامات مورد نیاز (dynamic list برای اقدامات مختلف مثل اسکن QR، تکمیل کوئیز، یا فعالیت Strike)
    public List<QuestAction> RequiredActions { get; private set; } = new List<QuestAction>();

    // پاداش‌ها (فقط پایه – XP نهایی در UserXp محاسبه می‌شه)
    public int XPReward { get; private set; } // XP پایه که با تکمیل quest داده می‌شه
    public int BonusXP { get; private set; } // XP اضافی برای golden level یا ایونت‌های فصلی

    // Level و اولویت
    public int Tier { get; private set; } // level/tier quest (مثل 1 برای ساده، 3 برای پیشرفته)
    public int Priority { get; private set; } // اولویت نمایش به کاربر (بالاتر = اول نمایش بده، e.g., 10 برای فصلی)

    // وضعیت عمومی quest (نه per-user)
    public QuestStatus Status { get; private set; } =
        QuestStatus.Pending; // enum برای وضعیت کلی (e.g., Active/Inactive)

    public DateTime? Deadline { get; private set; } // مهلت تکمیل (برای questهای فصلی)

    // لینک به اشیاء (از SQL)
    public List<long> RelatedObjectIds { get; private set; } =
        new List<long>(); // لینک به Object.Id در SQL (اشیاء موزه/QR Codes)

    // اتصال به فیچرهای Artix
    public bool IsSeasonal { get; private set; } // آیا بخشی از ایونت فصلی مثل نوروز هست؟
    public string RelatedFeature { get; private set; } // مثل "QRHunts", "CoUp", "Strike", "LastQuiz"

    // Constructor برای ایجاد quest جدید (بدون UserId – عمومی)
    protected Quest()
    {
    } // برای serialization

    public Quest(string title, string description, int xpReward, int bonusXp, int tier, int priority, DateTime? deadline = null, bool isSeasonal = false, string relatedFeature = null)
    {
        Title = title;
        Description = description;
        XPReward = xpReward;
        BonusXP = bonusXp;
        Tier = tier;
        Priority = priority;
        Deadline = deadline;
        IsSeasonal = isSeasonal;
        RelatedFeature = relatedFeature;
        RaiseDomainEvent(new QuestCreatedEvent(BusinessId));
    }

    public void AddAction(string actionType, string details, int requiredCount)
    {
        RequiredActions.Add(new QuestAction(actionType, details, requiredCount));
    }

 
}
