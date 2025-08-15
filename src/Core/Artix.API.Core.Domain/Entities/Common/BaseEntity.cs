namespace Artix.API.Core.Domain.Entities.Common;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class BaseEntity
{
    [Key]
    public long Id { get; protected set; } 
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; protected set; }
    public bool IsDeleted { get; protected set; } = false;
    public Guid BusinessId { get; protected set; } = Guid.CreateVersion7();
    
    
    // برای پشتیبانی از EF Core
    protected BaseEntity() { }

    // متدهای اختیاری برای رفتارهای مشترک
    protected void MarkAsDeleted()
    {
        IsDeleted = true;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkAsModified()
    {
        IsDeleted = false;
        ModifiedAt = DateTime.UtcNow;
    }
}
