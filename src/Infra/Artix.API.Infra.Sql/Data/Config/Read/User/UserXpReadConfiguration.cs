namespace Artix.API.Infra.Sql.Data.Config.Read.User;

using Artix.API.Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserXpReadConfiguration : BaseEntityConfiguration<UserXp> 
{
    public void Configure(EntityTypeBuilder<UserXp> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserXps");

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.TotalXp)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.LastUpdated)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        entity.HasOne(e => e.User)
            .WithMany(u => u.UserXps)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasCheckConstraint("CK_UserXps_TotalXp_NonNegative", "[TotalXp] >= 0"); // Ensure TotalXp is non-negative

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserXps_UserId")
            .IsUnique();

        entity.HasIndex(e => e.TotalXp)
            .HasDatabaseName("IX_UserXps_TotalXp");

        entity.HasIndex(e => e.LastUpdated)
            .HasDatabaseName("IX_UserXps_LastUpdated");
    }
}
