namespace Artix.API.Core.Domain.Entities.Quiz.Events;

using DomainEvents;

public sealed class QuizCreatedEvent : IDomainEvent
{
    public Guid QuestId { get; }
    public QuizCreatedEvent(Guid questId) { this.QuestId = questId; }
}
