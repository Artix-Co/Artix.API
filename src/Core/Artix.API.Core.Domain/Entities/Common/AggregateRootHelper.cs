namespace Artix.API.Core.Domain.Entities.Common;


public sealed class AggregateRootHelper
{
    private readonly List<BaseEntity> _entities = new();

    public IReadOnlyCollection<BaseEntity> Entities => _entities.AsReadOnly();

    public void AddEntity(BaseEntity entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        _entities.Add(entity);
    }

    public void RemoveEntity(BaseEntity entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        _entities.Remove(entity);
    }

    public IEnumerable<T> GetEntitiesOfType<T>() where T : BaseEntity
    {
        return _entities.OfType<T>();
    }

    public void ClearEntities()
    {
        _entities.Clear();
    }
}
