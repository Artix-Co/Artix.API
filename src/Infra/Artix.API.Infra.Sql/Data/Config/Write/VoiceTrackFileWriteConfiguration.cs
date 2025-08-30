namespace Artix.API.Infra.Sql.Data.Config.Write;
using Core.Domain.Entities.Voice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class VoiceTrackFileWriteConfiguration : IEntityTypeConfiguration<VoiceTrackFile>
{
    public void Configure(EntityTypeBuilder<VoiceTrackFile> entity)
    {
        entity.ToTable("VoiceTrackFiles");

        entity.HasKey(vtf => new { vtf.FileId, vtf.VoiceTrackId });

        entity.Property(vtf => vtf.FileId).IsRequired();
        entity.Property(vtf => vtf.VoiceTrackId).IsRequired();

        entity.HasOne(vtf => vtf.FileEntity)
            .WithMany(f => f.VoiceTrackFiles)
            .HasForeignKey(vtf => vtf.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(vtf => vtf.VoiceTrack)
            .WithMany(o => o.VoiceTrackFiles)
            .HasForeignKey(vtf => vtf.VoiceTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(vtf => vtf.FileId)
            .HasDatabaseName("IX_VoiceTrackFiles_FileId");

        entity.HasIndex(vtf => vtf.VoiceTrackId)
            .HasDatabaseName("IX_VoiceTrackFiles_VoiceTrackId");
    }
}
