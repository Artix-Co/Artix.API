namespace Artix.API.Infra.Sql.Data.Config.Write.Voice;

using Artix.API.Core.Domain.Entities.Voice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class VoiceTrackWriteConfiguration : BaseEntityConfiguration<VoiceTrack>
{
    public override void Configure(EntityTypeBuilder<VoiceTrack> entity)
    {
        base.Configure(entity);

        entity.ToTable("VoiceTracks");

        entity.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired(false);

        entity.Property(e => e.Artist)
            .HasMaxLength(100)
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

        entity.HasOne(e => e.Object)
            .WithMany()
            .HasForeignKey(e => e.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasMany(o => o.VoiceTrackFiles)
            .WithOne(ot => ot.VoiceTrack)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        

        entity.HasIndex(e => e.SeasonId)
            .HasDatabaseName("IX_MusicTracks_SeasonId");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("IX_MusicTracks_Title");

        entity.HasIndex(e => e.Artist)
            .HasDatabaseName("IX_MusicTracks_Artist");
    }
}
