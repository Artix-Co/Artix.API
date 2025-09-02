namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumObjectWriteConfiguration : IEntityTypeConfiguration<MuseumObject>
{
    public void Configure(EntityTypeBuilder<MuseumObject> entity)
    {
        entity.ToTable("MuseumObjects");

        entity.HasKey(of => new { of.MuseumId, of.ObjectId });

        entity.Property(of => of.MuseumId).IsRequired();
        entity.Property(of => of.ObjectId).IsRequired();

        // Relationships
        entity
            .HasOne(mo => mo.Museum)
            .WithMany(m => m.MuseumObjects)
            .HasForeignKey(mo => mo.MuseumId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(mo => mo.Object)
            .WithMany()
            .HasForeignKey(mo => mo.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(mo => mo.MuseumId)
            .HasDatabaseName("IX_MuseumObject_MuseumId");

        entity.HasIndex(mo => mo.ObjectId)
            .HasDatabaseName("IX_MuseumObject_ObjectId");
    }
}
