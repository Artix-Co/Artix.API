namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Microsoft.AspNetCore.Identity;

public sealed class AppRole : IdentityRole<long>,IAggregateRoot
{
    private readonly AggregateRootHelper _aggregate = new();

    public IReadOnlyCollection<BaseEntity> Entities => _aggregate.Entities;
    public void AddEntity(BaseEntity entity) => _aggregate.AddEntity(entity);
    public void RemoveEntity(BaseEntity entity) => _aggregate.RemoveEntity(entity);
    public IEnumerable<T> GetEntitiesOfType<T>() where T : BaseEntity => _aggregate.GetEntitiesOfType<T>();
    public void ClearEntities() => _aggregate.ClearEntities();
    
    
    public AppRole(string roleName) : base(roleName)
    {
    }

    public AppRole() : base()
    {
    }
}
