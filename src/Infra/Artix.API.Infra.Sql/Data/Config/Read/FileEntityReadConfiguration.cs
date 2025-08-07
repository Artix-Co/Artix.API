namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class FileEntityReadConfiguration : BaseEntityConfiguration<FileEntity>
{

    public override void Configure(EntityTypeBuilder<FileEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("Files");

        // Properties
        entity.Property(f => f.EntityType)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(f => f.EntityId)
            .IsRequired();

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

        entity.Property(f => f.UploadedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        entity.Property(f => f.UploadedBy)
            .HasMaxLength(100)
            .IsRequired(false);

 
        
        
        // Check Constraint
        entity.ToTable(t => t.HasCheckConstraint("CHK_EntityType", "[EntityType] IN ('Object', 'MusicTrack')"));

        // Indexes
        entity.HasIndex(f => new { f.EntityType, f.EntityId })
            .HasDatabaseName("IX_Files_EntityType_EntityId");
    }
}
