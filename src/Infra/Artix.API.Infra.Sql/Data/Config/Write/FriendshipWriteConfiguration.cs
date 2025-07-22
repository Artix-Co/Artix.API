namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class FriendshipWriteConfiguration : BaseEntityConfiguration<Friendship>,
    IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> entity)
    {
        base.Configure(entity);

        entity.ToTable("Friendships");

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.FriendId)
            .IsRequired();

        entity.HasOne(e => e.User)
            .WithMany(u => u.FriendshipUsers)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete Friendship when User is deleted

        entity.HasOne(e => e.Friend)
            .WithMany(u => u.FriendshipFriends)
            .HasForeignKey(e => e.FriendId)
            .OnDelete(DeleteBehavior.Cascade); // Delete Friendship when Friend is deleted

        entity.HasCheckConstraint("CK_Friendships_UserId_NotEqual_FriendId",
            "[UserId] != [FriendId]"); // Prevent self-friendship

        entity.HasIndex(e => new { e.UserId, e.FriendId })
            .HasDatabaseName("IX_Friendships_UserId_FriendId")
            .IsUnique();

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Friendships_UserId");

        entity.HasIndex(e => e.FriendId)
            .HasDatabaseName("IX_Friendships_FriendId");
    }
}
