namespace Artix.API.Infra.Sql.Data.Config.Write.Object;

using Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class ObjectSpecialInformationWriteConfiguration: IEntityTypeConfiguration<ObjectSpecialInformation>
{
    public void Configure(EntityTypeBuilder<ObjectSpecialInformation> entity)
    {
        entity.ToTable("ObjectSpecialInformation");

        entity.HasKey(of => new { of.FileId, of.ObjectId });

        entity.Property(of => of.FileId).IsRequired();
        entity.Property(of => of.ObjectId).IsRequired();

        entity.HasOne(of => of.FileEntity)
            .WithMany(f => f.ObjectSpecialInformation)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(of => of.Object)
            .WithMany(o => o.ObjectSpecialInformation)
            .HasForeignKey(of => of.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(of => of.FileId)
            .HasDatabaseName("IX_ObjectSpecialInformationFiles_FileId");

        entity.HasIndex(of => of.ObjectId)
            .HasDatabaseName("IX_ObjectSpecialInformationFiles_ObjectId");
    }
}
