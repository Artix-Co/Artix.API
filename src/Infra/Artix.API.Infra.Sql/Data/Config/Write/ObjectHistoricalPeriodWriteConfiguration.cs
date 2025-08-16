namespace Artix.API.Infra.Sql.Data.Config.Write;

using Artix.API.Core.Domain.Entities.Museum;
using Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ObjectHistoricalPeriodWriteConfiguration : IEntityTypeConfiguration<ObjectHistoricalPeriod>
{
    public void Configure(EntityTypeBuilder<ObjectHistoricalPeriod> entity)
    {
        entity.ToTable("ObjectHistoricalPeriods");

        // Composite primary key
        entity.HasKey(ohp => new { ohp.ObjectId, ohp.HistoricalPeriodId });

        // Relationships
        entity
            .HasOne(ohp => ohp.Object)
            .WithMany(o => o.ObjectHistoricalPeriods)
            .HasForeignKey(ohp => ohp.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(ohp => ohp.HistoricalPeriod)
            .WithMany(hp => hp.ObjectHistoricalPeriods)
            .HasForeignKey(ohp => ohp.HistoricalPeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(ohp => ohp.ObjectId)
            .HasDatabaseName("IX_ObjectHistoricalPeriods_ObjectId");

        entity.HasIndex(ohp => ohp.HistoricalPeriodId)
            .HasDatabaseName("IX_ObjectHistoricalPeriods_HistoricalPeriodId");
    }
}
