namespace Artix.API.Core.Domain.Entities.Common;

using Exceptions;
using Microsoft.EntityFrameworkCore;

public abstract class BaseAggregateRoot : IAggregateRoot, IEntity
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected init; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; protected set; }
    public bool IsDeleted { get; protected set; } = false;
    public Guid BusinessId { get; protected set; } = Guid.CreateVersion7();

    private readonly List<IEntity> _entities = new();
    public IReadOnlyCollection<IEntity> Entities => _entities.AsReadOnly();

    public void AddEntity(IEntity entity)
    {
        if (entity == null) throw DomainException.InvalidValue(nameof(entity));
        _entities.Add(entity);
        SetModified();
    }

    public void RemoveEntity(IEntity entity)
    {
        if (entity == null) throw DomainException.InvalidValue(nameof(entity));
        _entities.Remove(entity);
        SetModified();
    }

    public IEnumerable<T> GetEntitiesOfType<T>() where T : IEntity
    {
        return _entities.OfType<T>();
    }

    public void ClearEntities()
    {
        _entities.Clear();
        SetModified();
    }

    protected void SetModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseAggregateRoot other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * 31;
    }

    public virtual void ApplyGraphTracking(DbContext context)
    {
        if (context.Entry(this).State == EntityState.Detached)
        {
            context.Entry(this).State = EntityState.Modified;
        }
    }
}
