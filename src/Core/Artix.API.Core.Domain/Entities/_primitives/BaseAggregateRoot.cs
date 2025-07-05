namespace Artix.API.Core.Domain.Entities._primitives;

public abstract class BaseAggregateRoot : BaseEntity
{
    private readonly List<BaseEntity> _entities = new();

    protected IReadOnlyCollection<BaseEntity> Entities => _entities.AsReadOnly();

    protected void AddEntity(BaseEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _entities.Add(entity);
        SetModified();
    }

    protected void RemoveEntity(BaseEntity entity)
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
}
