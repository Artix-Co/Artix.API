namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserTrackReadConfiguration : BaseEntityConfiguration<UserTrack> 
{
    public void Configure(EntityTypeBuilder<UserTrack> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserTracks");

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.TrackId)
            .IsRequired();

        entity.Property(e => e.AcquiredAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        entity.HasOne(e => e.User)
            .WithMany(u => u.UserTracks)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Track)
            .WithMany()
            .HasForeignKey(e => e.TrackId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasCheckConstraint("CK_UserTracks_UserId_NotEqual_TrackId",
            "[UserId] != [TrackId]"); // Prevent invalid relationships

        entity.HasIndex(e => new { e.UserId, e.TrackId })
            .HasDatabaseName("IX_UserTracks_UserId_TrackId")
            .IsUnique();

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserTracks_UserId");

        entity.HasIndex(e => e.TrackId)
            .HasDatabaseName("IX_UserTracks_TrackId");

        entity.HasIndex(e => e.AcquiredAt)
            .HasDatabaseName("IX_UserTracks_AcquiredAt");
    }
}
