namespace Artix.API.Infra.Sql.Data.Config;

using Core.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnType("smalldatetime")
            .HasConversion(
                v => EnsureSmallDateTimeRange(v),
                v => v);

        builder.Property(e => e.ModifiedAt)
            .HasColumnType("smalldatetime")
            .HasConversion(
                v => v.HasValue ? EnsureSmallDateTimeRange(v.Value) : (DateTime?)null,
                v => v);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.BusinessId)
            .IsRequired();

        // Ignore Entities collection if applicable
        var entitiesProp = typeof(TEntity).GetProperty("Entities");
        if (entitiesProp is not null)
            builder.Ignore("Entities");
    }

    private static DateTime EnsureSmallDateTimeRange(DateTime date)
    {
        var min = new DateTime(1900, 1, 1);
        var max = new DateTime(2079, 6, 6);
        return date < min ? min : date > max ? max : date;
    }
}
