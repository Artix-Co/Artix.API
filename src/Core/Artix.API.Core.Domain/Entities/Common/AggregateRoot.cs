namespace Artix.API.Core.Domain.Entities.Common;

using DomainEvents;

public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
        // به‌روزرسانی ModifiedAt برای ردیابی تغییرات
        ModifiedAt = DateTime.UtcNow;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
