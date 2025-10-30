namespace Artix.API.Core.Domain.Entities.Quiz.Events;

using Artix.API.Core.Domain.DomainEvents;

public sealed class QuizCompletedEvent : IDomainEvent
{
    public long QuestId { get; }
    public long UserId { get; }
    public int XPReward { get; }

    public QuizCompletedEvent(long questId, long userId, int xpReward)
    {
        this.QuestId = questId;
        this.UserId = userId;
        this.XPReward = xpReward;
    }
}
