namespace Artix.API.Infra.Sql.Data.Config.Read.User;

using Artix.API.Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserMuseumKeyReadConfiguration : BaseEntityConfiguration<UserMuseumKey> 
{
    public void Configure(EntityTypeBuilder<UserMuseumKey> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserMuseumKeys");

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.MuseumId)
            .IsRequired();

        entity.Property(e => e.UnlockedAt)
            .IsRequired(false);

        entity.HasOne(e => e.User)
            .WithMany(u => u.UserMuseumKeys)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Museum)
            .WithMany()
            .HasForeignKey(e => e.MuseumId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasCheckConstraint("CK_UserMuseumKeys_UserId_NotEqual_MuseumId",
            "[UserId] != [MuseumId]"); // Prevent invalid relationships

        entity.HasIndex(e => new { e.UserId, e.MuseumId })
            .HasDatabaseName("IX_UserMuseumKeys_UserId_MuseumId")
            .IsUnique();

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserMuseumKeys_UserId");

        entity.HasIndex(e => e.MuseumId)
            .HasDatabaseName("IX_UserMuseumKeys_MuseumId");

        entity.HasIndex(e => e.UnlockedAt)
            .HasDatabaseName("IX_UserMuseumKeys_UnlockedAt");
    }
}
