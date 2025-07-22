namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserJournalEntryWriteConfiguration : BaseEntityConfiguration<UserJournalEntry>,
    IEntityTypeConfiguration<UserJournalEntry>
{
    public void Configure(EntityTypeBuilder<UserJournalEntry> entity)
    {
        base.Configure(entity);

        entity.ToTable("UserJournalEntries");

        entity.Property(e => e.UnlockedAt)
            .IsRequired(false);

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.EntryId)
            .IsRequired();

        entity.HasOne(e => e.User)
            .WithMany(u => u.UserJournalEntries)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Entry)
            .WithMany(je => je.UserJournalEntries)
            .HasForeignKey(e => e.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasCheckConstraint("CK_UserJournalEntries_UserId_NotEqual_EntryId",
            "[UserId] != [EntryId]"); // Prevent invalid relationships (if applicable)

        entity.HasIndex(e => new { e.UserId, e.EntryId })
            .HasDatabaseName("IX_UserJournalEntries_UserId_EntryId")
            .IsUnique();

        entity.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserJournalEntries_UserId");

        entity.HasIndex(e => e.EntryId)
            .HasDatabaseName("IX_UserJournalEntries_EntryId");

        entity.HasIndex(e => e.UnlockedAt)
            .HasDatabaseName("IX_UserJournalEntries_UnlockedAt");
    }
}
