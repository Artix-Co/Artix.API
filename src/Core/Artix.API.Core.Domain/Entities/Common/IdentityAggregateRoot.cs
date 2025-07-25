namespace Artix.API.Core.Domain.Entities.Common;

using Exceptions;
using Microsoft.AspNetCore.Identity;

public abstract class IdentityAggregateRoot : IdentityUser<long>, IAggregateRoot, IEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Guid BusinessId { get; set; } = Guid.CreateVersion7();
    
    
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
        if (entity == null) throw new ArgumentNullException(nameof(entity));
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
}
