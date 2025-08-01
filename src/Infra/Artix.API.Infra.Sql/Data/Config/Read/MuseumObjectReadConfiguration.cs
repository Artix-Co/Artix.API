namespace Artix.API.Infra.Sql.Data.Config.Read;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumObjectReadConfiguration : BaseEntityConfiguration<MuseumObject>
{
    public void Configure(EntityTypeBuilder<MuseumObject> entity)
    {
        base.Configure(entity);

        entity.ToTable("MuseumObjects");

        entity.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(e => e.Version)
            .IsRequired(false);

        entity.Property(e => e.Tier)
            .IsRequired(false);

        entity.Property(e => e.MuseumId)
            .IsRequired();

        entity.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.QRCode)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.IsSpecial)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);

       entity
            .HasOne(mo => mo.Museum)
            .WithMany(m => m.MuseumObjects)
            .HasForeignKey(mo => mo.MuseumId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        entity.HasMany(e => e.MuseumObjectCategories)
            .WithOne(moc => moc.MuseumObject)
            .HasForeignKey(moc => moc.MuseumObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.MuseumId)
            .HasDatabaseName("IX_MuseumObjects_MuseumId");

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_MuseumObjects_Name");

        entity.HasIndex(e => e.QRCode)
            .HasDatabaseName("IX_MuseumObjects_QRCode")
            .IsUnique();
    }

}
