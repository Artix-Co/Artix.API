namespace Artix.API.Core.Domain.Entities.Common;

public interface IAggregateRoot
{
    IReadOnlyCollection<BaseEntity> Entities { get; }
    void AddEntity(BaseEntity entity);
    void RemoveEntity(BaseEntity entity);
    IEnumerable<T> GetEntitiesOfType<T>() where T : BaseEntity;
    void ClearEntities();
}
