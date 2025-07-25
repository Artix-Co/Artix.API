namespace Artix.API.Infra.Sql.Data.Config;

using Core.Domain.Entities._primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).IsRequired();

     
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
    }

    private static DateTime EnsureSmallDateTimeRange(DateTime date)
    {
        DateTime minSqlDate = new DateTime(1900, 1, 1);
        DateTime maxSqlDate = new DateTime(2079, 6, 6);

        if (date < minSqlDate)
            return minSqlDate;
        else if (date > maxSqlDate)
            return maxSqlDate;

        return date;
    }
}
