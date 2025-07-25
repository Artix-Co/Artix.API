namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CategoryWriteConfiguration : BaseEntityConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        base.Configure(entity);

        entity.ToTable("Categories");

        entity.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        entity.HasMany(e => e.MuseumObjectCategories)
            .WithOne(moc => moc.Category)
            .HasForeignKey(moc => moc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Categories_Name")
            .IsUnique();
    }
}
