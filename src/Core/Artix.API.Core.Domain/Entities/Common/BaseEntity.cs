namespace Artix.API.Core.Domain.Entities.Common;

using System.ComponentModel.DataAnnotations;

public abstract class BaseEntity : IEntity
{
    [Key]
    public long Id { get; set; }
    public DateTime CreatedAt { get; protected init; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Guid BusinessId { get; set; } = Guid.CreateVersion7();

    public virtual void SetModified()
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
