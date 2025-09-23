namespace Artix.API.Infra.Sql.Data.Config;

using Core.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedOnAdd(); // Assumes Id is auto-generated (e.g., Guid or int)

        // DateTime Properties
        builder.Property(e => e.CreatedAt)
            .HasColumnType("smalldatetime")
            .IsRequired()
            .HasConversion(
                v => EnsureSmallDateTimeRange(v),
                v => v);

        builder.Property(e => e.ModifiedAt)
            .HasColumnType("smalldatetime")
            .HasConversion(
                v => v.HasValue ? EnsureSmallDateTimeRange(v.Value) : (DateTime?)null,
                v => v);

        // Soft Delete Property
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // BusinessId Property
        builder.Property(e => e.BusinessId)
            .IsRequired();

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Indexes for Performance
        builder.HasIndex(e => e.BusinessId)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_BusinessId");

        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");

        // Composite Index for Common Query Pattern (optional, adjust based on usage)
        builder.HasIndex(e => new { e.BusinessId, e.IsDeleted })
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_BusinessId_IsDeleted");

        // Optional: Index on CreatedAt for sorting or filtering
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedAt");

        // Ignore Entities collection if applicable
        var entitiesProp = typeof(TEntity).GetProperty("Entities");
        if (entitiesProp != null)
            builder.Ignore("Entities");

        // Allow derived classes to add custom configurations
        ConfigureDerived(builder);
    }

    /// <summary>
    /// Ensures DateTime values fit within smalldatetime range (1900-01-01 to 2079-06-06).
    /// </summary>
    private static DateTime EnsureSmallDateTimeRange(DateTime date)
    {
        var min = new DateTime(1900, 1, 1);
        var max = new DateTime(2079, 6, 6);
        return date < min ? min : date > max ? max : date;
    }

    /// <summary>
    /// Allows derived classes to add custom configurations.
    /// </summary>
    protected virtual void ConfigureDerived(EntityTypeBuilder<TEntity> builder)
    {
        // Override in derived classes for additional configurations
    }
}
