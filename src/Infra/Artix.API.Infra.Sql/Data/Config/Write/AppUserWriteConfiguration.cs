namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AppUserWriteConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> entity)
    {
        entity.ToTable("AppUsers");

        entity.Property(e => e.BusinessId)
            .IsRequired()
            .HasDefaultValueSql("NEWID()");

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        entity.Property(e => e.ModifiedAt).IsRequired(false);
        entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired(false);
        entity.Property(e => e.AvatarUrl).HasMaxLength(2000).IsRequired(false);
        entity.Property(e => e.IsPro).IsRequired().HasDefaultValue(false);

        entity.HasMany(e => e.Collections)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.FriendshipFriends)
            .WithOne(f => f.Friend)
            .HasForeignKey("FriendId")
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.FriendshipUsers)
            .WithOne(f => f.User)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.MarketplaceItems)
            .WithOne(mi => mi.Seller)
            .HasForeignKey(mi => mi.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.UserJournalEntries)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.UserMuseumKeys)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.UserObjects)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.UserSeasonProgresses)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.UserStrikes)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.UserTracks)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.UserXps)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.BusinessId)
            .HasDatabaseName("IX_AppUsers_BusinessId")
            .IsUnique();

        entity.HasIndex(e => e.DisplayName)
            .HasDatabaseName("IX_AppUsers_DisplayName");

        entity.HasIndex(e => e.IsPro)
            .HasDatabaseName("IX_AppUsers_IsPro");

        entity.HasIndex(e => e.IsDeleted)
            .HasDatabaseName("IX_AppUsers_IsDeleted");
    }
}
