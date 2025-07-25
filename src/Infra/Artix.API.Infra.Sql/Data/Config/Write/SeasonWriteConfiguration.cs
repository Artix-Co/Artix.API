namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.Season;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class SeasonWriteConfiguration : BaseEntityConfiguration<Season> 
{
    public void Configure(EntityTypeBuilder<Season> entity)
    {
        base.Configure(entity);

        entity.ToTable("Seasons");

        entity.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired(false);

        entity.Property(e => e.StartDate)
            .IsRequired(false);

        entity.Property(e => e.EndDate)
            .IsRequired(false);

        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        entity.HasMany(e => e.SeasonTasks)
            .WithOne()
            .HasForeignKey("SeasonId")
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.UserSeasonProgresses)
            .WithOne()
            .HasForeignKey("SeasonId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Seasons_Name")
            .IsUnique();

        entity.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Seasons_IsActive");

        entity.HasIndex(e => e.StartDate)
            .HasDatabaseName("IX_Seasons_StartDate");
    }
}
