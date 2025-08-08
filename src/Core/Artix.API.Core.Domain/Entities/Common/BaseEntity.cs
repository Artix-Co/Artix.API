namespace Artix.API.Core.Domain.Entities.Common;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class BaseEntity
{
    [Key]
    public long Id { get; private set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Guid BusinessId { get; private set; } = Guid.CreateVersion7();
}
