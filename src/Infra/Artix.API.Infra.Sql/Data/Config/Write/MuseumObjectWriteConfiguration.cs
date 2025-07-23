namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class MuseumObjectWriteConfiguration: BaseEntityConfiguration<MuseumObject>,
    IEntityTypeConfiguration<MuseumObject>
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

        entity.HasOne(e => e.Museum)
            .WithMany(m => m.MuseumObjects)
            .HasForeignKey(e => e.MuseumId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(e => e.MuseumId)
            .HasDatabaseName("IX_MuseumObjects_MuseumId");

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_MuseumObjects_Name");

        entity.HasIndex(e => e.QRCode)
            .HasDatabaseName("IX_MuseumObjects_QRCode")
            .IsUnique(); 
    }

}
