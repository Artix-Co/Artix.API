namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumObjectReadConfiguration : BaseEntityConfiguration<MuseumObject>
{
    public override void Configure(EntityTypeBuilder<MuseumObject> entity)
    {
        base.Configure(entity);

        entity.ToTable("MuseumObjects");

        // Properties
        entity.Property(mo => mo.MuseumId)
            .IsRequired();

        entity.Property(mo => mo.ObjectId)
            .IsRequired();

        // Relationships
        entity
            .HasOne(mo => mo.Museum)
            .WithMany(m => m.MuseumObjects)
            .HasForeignKey(mo => mo.MuseumId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(mo => mo.Object)
            .WithMany() // No navigation property in Object for MuseumObjects
            .HasForeignKey(mo => mo.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(mo => mo.MuseumId)
            .HasDatabaseName("IX_MuseumObjects_MuseumId");

        entity.HasIndex(mo => mo.ObjectId)
            .HasDatabaseName("IX_MuseumObjects_ObjectId")
            .IsUnique(); // Assuming each Object can be linked to only one Museum
    }
}
