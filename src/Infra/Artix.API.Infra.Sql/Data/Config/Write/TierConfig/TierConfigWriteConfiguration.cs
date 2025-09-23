namespace Artix.API.Infra.Sql.Data.Config.Write.TierConfig;

using Core.Domain.Entities.TierConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class TierConfigWriteConfiguration : BaseEntityConfiguration<TierConfig>
{
    public void Configure(EntityTypeBuilder<TierConfig> builder)
    {
        builder.ToTable("TierConfigs");
        
        builder.Property(x => x.MinScanCount)
            .IsRequired();

        builder.Property(x => x.RequiredUpgraded);

        builder.Property(x => x.RequiredInCollection);

        builder.Property(x => x.MinDaysSinceAcquired);

        builder.Property(x => x.RequiredSpecial);

        builder.Property(x => x.RequiredSaleType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.RequiredMembershipType)
            .HasMaxLength(100);

        builder.Property(x => x.RequiredActiveStreak);

        builder.Property(x => x.RequiredCoOpKey);

        builder.Property(x => x.TierLevel)
            .IsRequired();

        builder.Property(x => x.Multiplier)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(x => x.Priority)
            .IsRequired();
    }
}
