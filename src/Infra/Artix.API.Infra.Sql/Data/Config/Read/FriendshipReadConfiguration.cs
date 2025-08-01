namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


internal sealed class FriendshipReadConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> entity)
    {
        entity.ToTable("Friendships");

        // Configure composite key
        entity.HasKey(f => new { f.UserId, f.FriendId });

        // Configure properties
        entity.Property(f => f.UserId).IsRequired();
        entity.Property(f => f.FriendId).IsRequired();
        entity.Property(f => f.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Configure User relationship
        entity.HasOne(f => f.User)
            .WithMany(u => u.FriendshipFriends)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Friend relationship
        entity.HasOne(f => f.Friend)
            .WithMany() // No inverse navigation for Friend
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        entity.HasIndex(f => f.UserId)
            .HasDatabaseName("IX_Friendships_UserId");

        entity.HasIndex(f => f.FriendId)
            .HasDatabaseName("IX_Friendships_FriendId");

        // Unique constraint (redundant with composite key but explicit for clarity)
        entity.HasIndex(f => new { f.UserId, f.FriendId })
            .IsUnique();

        // Prevent self-friendships
        entity.HasCheckConstraint("CK_Friendships_NotSelf", "[UserId] <> [FriendId]");
    }
}
