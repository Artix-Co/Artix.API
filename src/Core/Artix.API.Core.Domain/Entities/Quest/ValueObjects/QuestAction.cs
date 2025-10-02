namespace Artix.API.Core.Domain.Entities.Quest.ValueObjects;

// کلاس کمکی QuestAction (unchanged)
public class QuestAction
{
    public string ActionType { get; private set; } // مثل "ScanQR", "CompleteQuiz", "MaintainStreak"
    public string Details { get; private set; } // جزئیات، مثل "اسکن QR شیء X"
    public int RequiredCount { get; private set; } // تعداد مورد نیاز، مثل 5 اسکن

    public QuestAction(string actionType, string details, int requiredCount)
    {
        ActionType = actionType;
        Details = details;
        RequiredCount = requiredCount;
    }
}

