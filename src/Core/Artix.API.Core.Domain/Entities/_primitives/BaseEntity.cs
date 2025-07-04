namespace Artix.API.Core.Domain.Entities._primitives;

using System.ComponentModel.DataAnnotations;

public abstract class BaseEntity
{
    [Key]
    public long Id { get; private set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; } = true;
    public Guid BusinessId { get; set; } = Guid.CreateVersion7();
}
