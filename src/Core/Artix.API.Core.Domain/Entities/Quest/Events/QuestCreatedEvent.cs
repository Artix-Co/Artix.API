namespace Artix.API.Core.Domain.Entities.Quest.Events;

using DomainEvents;

// Domain Events نمونه (اصلاح‌شده بدون UserId اگر عمومی باشه)
public class QuestCreatedEvent : IDomainEvent
{
    public Guid QuestId { get; }
    public QuestCreatedEvent(Guid questId) { QuestId = questId; }
}
