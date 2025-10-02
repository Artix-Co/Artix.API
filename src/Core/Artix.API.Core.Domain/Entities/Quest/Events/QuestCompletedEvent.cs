namespace Artix.API.Core.Domain.Entities.Quest.Events;

using DomainEvents;

public class QuestCompletedEvent : IDomainEvent
{
    public long QuestId { get; }
    public long UserId { get; }
    public int XPReward { get; }

    public QuestCompletedEvent(long questId, long userId, int xpReward)
    {
        QuestId = questId;
        UserId = userId;
        XPReward = xpReward;
    }
}
