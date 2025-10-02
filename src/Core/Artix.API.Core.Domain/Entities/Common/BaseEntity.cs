namespace Artix.API.Core.Domain.Entities.Common;

using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public abstract class BaseEntity
{
    [Key] // برای EF Core
    public long Id { get; protected set; } // برای SQL Server

    [BsonId] // برای MongoDB
    [BsonRepresentation(BsonType.String)] // ذخیره Guid به صورت رشته
    public Guid BusinessId { get; protected set; }

    public bool IsDeleted { get; protected set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }

    protected BaseEntity()
    {
        BusinessId = Guid.CreateVersion7(); // مقداردهی منحصربه‌فرد برای MongoDB
        CreatedAt = DateTime.UtcNow;
    }

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
