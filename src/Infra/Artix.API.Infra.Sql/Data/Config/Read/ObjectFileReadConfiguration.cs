namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.Museum;
using Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ObjectFileReadConfiguration : IEntityTypeConfiguration<ObjectFile>
{
    public void Configure(EntityTypeBuilder<ObjectFile> entity)
    {
        entity.ToTable("ObjectFiles");

        entity.HasKey(of => new { of.FileId, of.ObjectId });

        entity.Property(of => of.FileId).IsRequired();
        entity.Property(of => of.ObjectId).IsRequired();

        entity.HasOne(of => of.File)
            .WithMany(f => f.ObjectFiles)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(of => of.Object)
            .WithMany(o => o.ObjectFiles)
            .HasForeignKey(of => of.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(of => of.FileId)
            .HasDatabaseName("IX_ObjectFiles_FileId");

        entity.HasIndex(of => of.ObjectId)
            .HasDatabaseName("IX_ObjectFiles_ObjectId");
    }
}
