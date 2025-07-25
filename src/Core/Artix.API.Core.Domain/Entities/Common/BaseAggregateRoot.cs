namespace Artix.API.Core.Domain.Entities.Common;


public abstract class BaseAggregateRoot : BaseEntity, IAggregateRoot, IEntity
{
    private readonly List<BaseEntity> _entities = new();

    public IReadOnlyCollection<BaseEntity> Entities => _entities.AsReadOnly();

    public void AddEntity(BaseEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _entities.Add(entity);
        SetModified();
    }

    public void RemoveEntity(BaseEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _entities.Remove(entity);
        SetModified();
    }

    public IEnumerable<T> GetEntitiesOfType<T>() where T : BaseEntity
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
}

