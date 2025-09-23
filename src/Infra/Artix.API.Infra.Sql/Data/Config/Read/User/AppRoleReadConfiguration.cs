namespace Artix.API.Infra.Sql.Data.Config.Read.User;

using Artix.API.Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AppRoleReadConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> entity)
    {

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(e => e.NormalizedName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(e => e.ConcurrencyStamp)
            .IsRequired(false);

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_AppRoles_Name");

        entity.HasIndex(e => e.NormalizedName)
            .HasDatabaseName("IX_AppRoles_NormalizedName")
            .IsUnique();
    }
}
