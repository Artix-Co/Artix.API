namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumImageReadConfiguration : IEntityTypeConfiguration<MuseumImage>
{
    public void Configure(EntityTypeBuilder<MuseumImage> entity)
    {
        entity.ToTable("MuseumImages");

        entity.HasKey(of => new { of.FileId, of.MuseumId });

        entity.Property(of => of.FileId).IsRequired();
        entity.Property(of => of.MuseumId).IsRequired();

        entity.HasOne(of => of.File)
            .WithMany(f => f.MuseumImages)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(of => of.Museum)
            .WithMany(o => o.MuseumImages)
            .HasForeignKey(of => of.MuseumId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(of => of.FileId)
            .HasDatabaseName("IX_ObjectFiles_FileId");

        entity.HasIndex(of => of.MuseumId)
            .HasDatabaseName("IX_ObjectFiles_ObjectId");
    }
}
