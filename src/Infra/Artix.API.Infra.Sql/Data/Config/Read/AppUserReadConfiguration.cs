namespace Artix.API.Infra.Sql.Data.Config.Read;

using Artix.API.Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AppUserReadConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> entity)
    {

        entity.Property(e => e.DisplayName)
            .HasMaxLength(100)
            .IsRequired(false);
 

        entity.Property(e => e.IsPro)
            .IsRequired()
            .HasDefaultValue(false);


        // Relationships
        entity.HasMany(e => e.Collections)
            .WithOne(c => c.User);

        entity.HasMany(e => e.MarketplaceItems)
            .WithOne(mi => mi.Seller);

        entity.HasMany(e => e.UserJournalEntries)
            .WithOne(e => e.User);

        entity.HasMany(e => e.UserMuseumKeys)
            .WithOne(e => e.User);

        entity.HasMany(e => e.UserObjects)
            .WithOne(e => e.User);

        entity.HasMany(e => e.UserSeasonProgresses)
            .WithOne(e => e.User);

        entity.HasMany(e => e.UserStrikes)
            .WithOne(e => e.User);
        

        entity.HasMany(e => e.UserXps)
            .WithOne(e => e.User);

        
        entity
            .HasMany<AppUserToken>()
            .WithOne()
            .HasForeignKey(t => t.UserId)
            .IsRequired();

        entity.HasIndex(e => e.DisplayName)
            .HasDatabaseName("IX_AppUsers_DisplayName");

        entity.HasIndex(e => e.IsPro)
            .HasDatabaseName("IX_AppUsers_IsPro");
    }
}
