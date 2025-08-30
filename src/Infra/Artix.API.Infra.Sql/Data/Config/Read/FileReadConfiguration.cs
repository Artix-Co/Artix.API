namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class FileReadConfiguration : BaseEntityConfiguration<File>
{
    public override void Configure(EntityTypeBuilder<File> entity)
    {
        base.Configure(entity);

        entity.ToTable("Files");


        entity.Property(f => f.FileName)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(f => f.FilePath)
            .HasMaxLength(500)
            .IsRequired();

        entity.Property(f => f.FileSize)
            .IsRequired();

        entity.Property(f => f.MimeType)
            .HasMaxLength(100)
            .IsRequired(false);

        entity.Property(f => f.UploadedBy)
            .HasMaxLength(100)
            .IsRequired(false);
        
        
        entity.HasMany(f => f.ObjectModels)
            .WithOne(of => of.File)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasMany(f => f.ObjectImages)
            .WithOne(of => of.File)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(f => f.MuseumImages)
            .WithOne(of => of.File)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Cascade);
        
              
        entity.HasMany(f => f.VoiceTrackFiles)
            .WithOne(of => of.File)
            .HasForeignKey(of => of.VoiceTrackId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
