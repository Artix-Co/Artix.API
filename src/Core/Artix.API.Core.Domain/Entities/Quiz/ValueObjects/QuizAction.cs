namespace Artix.API.Core.Domain.Entities.Quiz.ValueObjects;


public sealed class QuizAction
{
    public string ActionType { get; private set; } // مثل "ScanQR", "CompleteQuiz", "MaintainStreak"
    public string Details { get; private set; } // جزئیات، مثل "اسکن QR شیء X"
    public int RequiredCount { get; private set; } // تعداد مورد نیاز، مثل 5 اسکن

    public QuizAction(string actionType, string details, int requiredCount)
    {
        this.ActionType = actionType;
        this.Details = details;
        this.RequiredCount = requiredCount;
    }
}

