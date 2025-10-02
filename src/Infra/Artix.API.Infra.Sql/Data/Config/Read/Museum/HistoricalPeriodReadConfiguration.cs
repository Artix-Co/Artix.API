namespace Artix.API.Infra.Sql.Data.Config.Read.Museum;

using Artix.API.Core.Domain.Entities.Museum;
using Core.Domain.Entities.Object.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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


 
// TODO: move it into utils/extensions
public class HistoricalDateConverter : ValueConverter<HistoricalDate?, string?>
{
    public HistoricalDateConverter()
        : base(
            historicalDate => historicalDate == null
                ? null
                : historicalDate.Year < 0
                    ? $"{Math.Abs(historicalDate.Year)} BC"
                    : $"{historicalDate.Year} AD",
            stringValue => string.IsNullOrEmpty(stringValue)
                ? null
                : stringValue.EndsWith("BC", StringComparison.OrdinalIgnoreCase)
                    ? new HistoricalDate(-int.Parse(stringValue.Replace(" BC", "", StringComparison.OrdinalIgnoreCase)), 1, 1)
                    : new HistoricalDate(int.Parse(stringValue.Replace(" AD", "", StringComparison.OrdinalIgnoreCase)), 1, 1))
    {
    }
}
