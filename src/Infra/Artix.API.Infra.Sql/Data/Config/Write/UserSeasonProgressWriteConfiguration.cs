namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserSeasonProgressWriteConfiguration : BaseEntityConfiguration<UserSeasonProgress>
{
    public void Configure(EntityTypeBuilder<UserSeasonProgress> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserSeasonProgresses");

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.SeasonId)
            .IsRequired();

        entity.Property(e => e.TotalXp)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.LastUpdated)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        entity.HasOne(e => e.User)
            .WithMany(u => u.UserSeasonProgresses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Season)
            .WithMany(s => s.UserSeasonProgresses)
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasCheckConstraint("CK_UserSeasonProgresses_TotalXp_NonNegative",
            "[TotalXp] >= 0"); // Ensure TotalXp is non-negative
        entity.HasCheckConstraint("CK_UserSeasonProgresses_UserId_NotEqual_SeasonId",
            "[UserId] != [SeasonId]"); // Prevent invalid relationships


        entity.HasIndex(e => new { e.UserId, e.SeasonId })
            .HasDatabaseName("IX_UserSeasonProgresses_UserId_SeasonId")
            .IsUnique();

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserSeasonProgresses_UserId");

        entity.HasIndex(e => e.SeasonId)
            .HasDatabaseName("IX_UserSeasonProgresses_SeasonId");

        entity.HasIndex(e => e.TotalXp)
            .HasDatabaseName("IX_UserSeasonProgresses_TotalXp");

        entity.HasIndex(e => e.LastUpdated)
            .HasDatabaseName("IX_UserSeasonProgresses_LastUpdated");
    }
}
