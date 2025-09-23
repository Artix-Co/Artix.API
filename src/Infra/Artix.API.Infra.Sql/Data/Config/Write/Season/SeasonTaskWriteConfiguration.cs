namespace Artix.API.Infra.Sql.Data.Config.Write.Season;

using Artix.API.Core.Domain.Entities.Season;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class SeasonTaskWriteConfiguration : BaseEntityConfiguration<SeasonTask> 
{
    public void Configure(EntityTypeBuilder<SeasonTask> entity)
    {
        base.Configure(entity);

        entity.ToTable("SeasonTasks");

        entity.Property(e => e.SeasonId)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        entity.Property(e => e.XpReward)
            .IsRequired();

        entity.Property(e => e.IsPro)
            .IsRequired()
            .HasDefaultValue(false);

        entity.HasOne(e => e.Season)
            .WithMany(s => s.SeasonTasks)
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(e => e.SeasonId)
            .HasDatabaseName("IX_SeasonTasks_SeasonId");

        entity.HasIndex(e => e.IsPro)
            .HasDatabaseName("IX_SeasonTasks_IsPro");
    }
}
