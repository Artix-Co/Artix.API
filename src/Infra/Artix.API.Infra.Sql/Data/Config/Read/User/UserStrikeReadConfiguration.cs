namespace Artix.API.Infra.Sql.Data.Config.Read.User;

using Artix.API.Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserStrikeReadConfiguration : BaseEntityConfiguration<UserStrike> 
{
    public void Configure(EntityTypeBuilder<UserStrike> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserStrikes");

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.StrikeStart)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        entity.Property(e => e.StrikeCount)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.LastInteraction)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        entity.HasOne(e => e.User)
            .WithMany(u => u.UserStrikes)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        


        entity.HasCheckConstraint("CK_UserStrikes_StrikeCount_NonNegative",
            "[StrikeCount] >= 0"); // Ensure StrikeCount is non-negative

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserStrikes_UserId");

        entity.HasIndex(e => e.StrikeStart)
            .HasDatabaseName("IX_UserStrikes_StrikeStart");

        entity.HasIndex(e => e.StrikeCount)
            .HasDatabaseName("IX_UserStrikes_StrikeCount");

        entity.HasIndex(e => e.LastInteraction)
            .HasDatabaseName("IX_UserStrikes_LastInteraction");
    }
}
