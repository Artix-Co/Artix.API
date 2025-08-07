namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Write;

internal sealed class HistoricalPeriodReadConfiguration : BaseEntityConfiguration<HistoricalPeriod>
{
    public override void Configure(EntityTypeBuilder<HistoricalPeriod> entity)
    {
        base.Configure(entity);

        entity.ToTable("HistoricalPeriods");

        // Properties
        entity.Property(hp => hp.Name)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(hp => hp.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(hp => hp.StartDate)
            .HasConversion<HistoricalDateConverter>()
            .HasColumnType("nvarchar(50)") // Store as string (e.g., "800 BC")
            .IsRequired(false);

        entity.Property(hp => hp.EndDate)
            .HasConversion<HistoricalDateConverter>()
            .HasColumnType("nvarchar(50)")
            .IsRequired(false);

        // Relationships
        entity.HasMany(hp => hp.ObjectHistoricalPeriods)
            .WithOne(ohp => ohp.HistoricalPeriod)
            .HasForeignKey(ohp => ohp.HistoricalPeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(hp => hp.Name)
            .HasDatabaseName("IX_HistoricalPeriods_Name")
            .IsUnique();
    }
}
