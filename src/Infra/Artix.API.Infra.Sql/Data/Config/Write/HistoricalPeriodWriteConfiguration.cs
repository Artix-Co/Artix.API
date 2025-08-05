namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class HistoricalPeriodWriteConfiguration : BaseEntityConfiguration<HistoricalPeriod>
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
            .IsRequired(false);

        entity.Property(hp => hp.EndDate)
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
