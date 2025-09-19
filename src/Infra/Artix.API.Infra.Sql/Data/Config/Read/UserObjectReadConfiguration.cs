namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserObjectReadConfiguration : BaseEntityConfiguration<UserScan> 
{
    public void Configure(EntityTypeBuilder<UserScan> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserObjects");

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.ObjectId)
            .IsRequired();

        entity.Property(e => e.ScanCount)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.AcquiredAt)
            .IsRequired(false);

        entity.Property(e => e.IsUpgraded)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.InCollection)
            .IsRequired()
            .HasDefaultValue(false);

        entity.HasOne(e => e.User)
            .WithMany(u => u.UserScans)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Object)
            .WithMany()
            .HasForeignKey(e => e.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasCheckConstraint("CK_UserObjects_ScanCount_NonNegative",
            "[ScanCount] >= 0"); // Ensure ScanCount is non-negative
        entity.HasCheckConstraint("CK_UserObjects_UserId_NotEqual_ObjectId",
            "[UserId] != [ObjectId]"); // Prevent invalid relationships

        entity.HasIndex(e => new { e.UserId, e.ObjectId })
            .HasDatabaseName("IX_UserObjects_UserId_ObjectId")
            .IsUnique();

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserObjects_UserId");

        entity.HasIndex(e => e.ObjectId)
            .HasDatabaseName("IX_UserObjects_ObjectId");

        entity.HasIndex(e => e.AcquiredAt)
            .HasDatabaseName("IX_UserObjects_AcquiredAt");

        entity.HasIndex(e => e.InCollection)
            .HasDatabaseName("IX_UserObjects_InCollection");
    }
}
