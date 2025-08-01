namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MusicTrackReadConfiguration : BaseEntityConfiguration<MusicTrack> 
{
    public void Configure(EntityTypeBuilder<MusicTrack> entity)
    {
        base.Configure(entity);

        entity.ToTable("MusicTracks");

        entity.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired(false);

        entity.Property(e => e.Artist)
            .HasMaxLength(100)
            .IsRequired(false);

        entity.Property(e => e.Url)
            .HasMaxLength(2000)
            .IsRequired(false);

        entity.Property(e => e.IsFree)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.SeasonId)
            .IsRequired(false);

        entity.HasOne(e => e.Season)
            .WithMany()
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);    
        
        
        entity.HasOne(e => e.MuseumObject)
            .WithMany()
            .HasForeignKey(e => e.MuseumObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.Tracks)
            .WithOne()
            .HasForeignKey("MusicTrackId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.SeasonId)
            .HasDatabaseName("IX_MusicTracks_SeasonId");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("IX_MusicTracks_Title");

        entity.HasIndex(e => e.Artist)
            .HasDatabaseName("IX_MusicTracks_Artist");
    }
}
