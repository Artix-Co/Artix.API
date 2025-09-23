namespace Artix.API.Infra.Sql.Data.Config.Read.Object;

using Artix.API.Core.Domain.Entities.Object;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class TypeReadConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> entity)
    {
        base.Configure(entity);

        entity.ToTable("Types");

        // Properties
        entity.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(t => t.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        // Relationships
        entity.HasMany(t => t.ObjectTypes)
            .WithOne(ot => ot.Category)
            .HasForeignKey(ot => ot.TypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(t => t.Name)
            .HasDatabaseName("IX_Types_Name")
            .IsUnique();
    }
}
