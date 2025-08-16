namespace Artix.API.Infra.Sql.Data.Config.Read;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Object = Core.Domain.Entities.Object.Object;

internal sealed class ObjectReadConfiguration : BaseEntityConfiguration<Object>
{
    public override void Configure(EntityTypeBuilder<Object> entity)
    {
        base.Configure(entity);

        entity.ToTable("Objects");

        // Properties
        entity.Property(o => o.Name)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(o => o.QrCode)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(o => o.GeneralInformation)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(o => o.SpecialInformation)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(o => o.Version)
            .IsRequired(false);

        entity.Property(o => o.Tier)
            .IsRequired(false);

        entity.Property(o => o.IsSpecial)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(o => o.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        entity.HasMany(o => o.ObjectTypes)
            .WithOne(ot => ot.Object)
            .HasForeignKey(ot => ot.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(o => o.ObjectHistoricalPeriods)
            .WithOne(ohp => ohp.Object)
            .HasForeignKey(ohp => ohp.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasMany(o => o.ObjectFiles)
            .WithOne(ot => ot.Object)
            .HasForeignKey(of => of.FileId)
            .OnDelete(DeleteBehavior.Cascade);


        // Indexes
        entity.HasIndex(o => o.Name)
            .HasDatabaseName("IX_Objects_Name");

        entity.HasIndex(o => o.QrCode)
            .HasDatabaseName("IX_Objects_QrCode")
            .IsUnique();
    }
}
