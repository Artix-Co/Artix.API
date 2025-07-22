namespace Artix.API.Core.Domain.Entities._primitives;

using Microsoft.EntityFrameworkCore;

public abstract class BaseEntity
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; } = true;
    public Guid BusinessId { get; set; } = Guid.CreateVersion7();

    public virtual void ApplyGraphTracking(DbContext context)
    {
        ApplyGraphTracking(context, new HashSet<object>());
    }

    
    private void ApplyGraphTracking(DbContext context, HashSet<object> visited)
    {
        if (visited.Contains(this))
            return;

        visited.Add(this);

        var entityType = context.Model.FindEntityType(this.GetType());
        if (entityType is null || entityType.IsOwned())
            return;

        context.Entry(this).State = EntityState.Modified;

        foreach (var navigation in context.Entry(this).Navigations)
        {
            var navValue = navigation.CurrentValue;
            if (navValue is null)
                continue;

            if (navigation.Metadata.IsCollection)
            {
                if (navValue is IEnumerable<object> collection)
                {
                    foreach (var item in collection)
                    {
                        if (item is null || visited.Contains(item))
                            continue;

                        var type = item.GetType();
                        if (context.Model.FindEntityType(type) is null)
                            continue;

                        context.Entry(item).State = EntityState.Modified;

                        if (item is BaseEntity nested)
                            nested.ApplyGraphTracking(context, visited);
                    }
                }
            }
            else
            {
                if (visited.Contains(navValue))
                    continue;

                var type = navValue.GetType();
                if (context.Model.FindEntityType(type) is null)
                    continue;

                context.Entry(navValue).State = EntityState.Modified;

                if (navValue is BaseEntity nested)
                    nested.ApplyGraphTracking(context, visited);
            }
        }
    }

    
    public void SetModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * 31;
    }
}
