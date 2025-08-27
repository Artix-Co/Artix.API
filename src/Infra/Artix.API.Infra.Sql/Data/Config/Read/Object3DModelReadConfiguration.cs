namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class Object3DModelReadConfiguration : IEntityTypeConfiguration<Object3DModel>
{
    public void Configure(EntityTypeBuilder<Object3DModel> entity)
    {
        entity.ToTable("Object3DModels");

        entity.HasKey(of => new { of.FileId, of.ObjectId });

        entity.Property(of => of.FileId).IsRequired();
        entity.Property(of => of.ObjectId).IsRequired();

        entity.HasOne(of => of.File)
            .WithMany(f => f.Object3DModels)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(of => of.Object)
            .WithMany(o => o.Object3DModels)
            .HasForeignKey(of => of.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(of => of.FileId)
            .HasDatabaseName("IX_ObjectFiles_FileId");

        entity.HasIndex(of => of.ObjectId)
            .HasDatabaseName("IX_ObjectFiles_ObjectId");
    }
}
