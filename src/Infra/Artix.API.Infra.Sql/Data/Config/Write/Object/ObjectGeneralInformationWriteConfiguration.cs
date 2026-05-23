namespace Artix.API.Infra.Sql.Data.Config.Write.Object;

using Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class ObjectGeneralInformationWriteConfiguration: IEntityTypeConfiguration<ObjectGeneralInformation>
{
    public void Configure(EntityTypeBuilder<ObjectGeneralInformation> entity)
    {
        entity.ToTable("ObjectGeneralInformation");

        entity.HasKey(of => new { of.FileId, of.ObjectId });

        entity.Property(of => of.FileId).IsRequired();
        entity.Property(of => of.ObjectId).IsRequired();

        entity.HasOne(of => of.FileEntity)
            .WithMany(f => f.ObjectGeneralInformation)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(of => of.Object)
            .WithMany(o => o.ObjectGeneralInformation)
            .HasForeignKey(of => of.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(of => of.FileId)
            .HasDatabaseName("IX_ObjectGeneralInformationFiles_FileId");

        entity.HasIndex(of => of.ObjectId)
            .HasDatabaseName("IX_ObjectGeneralInformationFiles_ObjectId");
    }
}
