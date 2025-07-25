namespace Artix.API.Core.Domain.Entities.Common;

public interface IAggregateRoot
{
    IReadOnlyCollection<IEntity> Entities { get; }
    void AddEntity(IEntity entity);
    void RemoveEntity(IEntity entity);
    IEnumerable<T> GetEntitiesOfType<T>() where T : IEntity;
    void ClearEntities();
}
