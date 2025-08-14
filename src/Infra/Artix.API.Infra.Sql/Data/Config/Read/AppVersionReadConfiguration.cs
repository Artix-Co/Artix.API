namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.Version;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AppVersionReadConfiguration : BaseEntityConfiguration<AppVersion>
{
    public override void Configure(EntityTypeBuilder<AppVersion> builder)
    {
        base.Configure(builder);

        builder.ToTable("AppVersions");

        builder.Property(v => v.Major)
            .IsRequired();

        builder.Property(v => v.Minor)
            .IsRequired();

        builder.Property(v => v.Patch)
            .IsRequired();

        builder.Property(v => v.VersionString)
            .HasComputedColumnSql(
                "CAST([Major] AS NVARCHAR(10)) + '.' + CAST([Minor] AS NVARCHAR(10)) + '.' + CAST([Patch] AS NVARCHAR(10))")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(v => v.IsRequired)
            .IsRequired();

        builder.Property(v => v.MinSupported)
            .IsRequired();

        builder.Property(v => v.Description)
            .IsRequired(false)
            .HasMaxLength(500);
    }
}
