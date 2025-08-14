namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Version;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AppVersionWriteConfiguration : BaseEntityConfiguration<AppVersion>
{
    public override void Configure(EntityTypeBuilder<AppVersion> entity)
    {
        base.Configure(entity);

        entity.ToTable("AppVersions");

        entity
            .Property(v => v.VersionString)
            .HasComputedColumnSql(
                "CAST([Major] AS NVARCHAR) + '.' + CAST([Minor] AS NVARCHAR) + '.' + CAST([Patch] AS NVARCHAR)");
    }
}
