namespace Artix.API.Infra.Sql.Data.Config.Write;

using Core.Domain.Entities.JournalEntry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class JournalEntryWriteConfiguration : BaseEntityConfiguration<JournalEntry>,
    IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> entity)
    {
        base.Configure(entity);

        entity.ToTable("JournalEntries");

        entity.Property(e => e.ObjectId)
            .IsRequired();

        entity.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired(false);

        entity.Property(e => e.SketchUrl)
            .HasMaxLength(2000)
            .IsRequired(false);

        entity.Property(e => e.Notes)
            .HasMaxLength(4000)
            .IsRequired(false);

        entity.HasOne(e => e.Object)
            .WithMany()
            .HasForeignKey(e => e.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.UserJournalEntries)
            .WithOne()
            .HasForeignKey("JournalEntryId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.ObjectId)
            .HasDatabaseName("IX_JournalEntries_ObjectId");

        entity.HasIndex(e => e.Title)
            .HasDatabaseName("IX_JournalEntries_Title");
    }
}
