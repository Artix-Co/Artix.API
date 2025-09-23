namespace Artix.API.Infra.Sql.Data.Config.Read.Object;

using Artix.API.Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ObjectModelReadConfiguration : IEntityTypeConfiguration<ObjectModel>
{
    public void Configure(EntityTypeBuilder<ObjectModel> entity)
    {
        entity.ToTable("ObjectModels");

        entity.HasKey(of => new { of.FileId, of.ObjectId });

        entity.Property(of => of.FileId).IsRequired();
        entity.Property(of => of.ObjectId).IsRequired();

        entity.HasOne(of => of.FileEntity)
            .WithMany(f => f.ObjectModels)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(of => of.Object)
            .WithMany(o => o.ObjectModels)
            .HasForeignKey(of => of.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(of => of.FileId)
            .HasDatabaseName("IX_ObjectModelFiles_FileId");

        entity.HasIndex(of => of.ObjectId)
            .HasDatabaseName("IX_ObjectModelFiles_ObjectId");
    }
}
