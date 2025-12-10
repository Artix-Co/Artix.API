namespace Artix.API.Infra.Sql.Data.Config.Write.OutboxMessages;

using Core.Domain.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class OutboxMessageWriteConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Type)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.Data)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(50)
            .HasDefaultValue("Pending")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .IsRequired(false);

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

        builder.Property(x => x.Error)
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty) // مهم: دیگر NULL نمی‌مونه
            .IsRequired();

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => new { x.Status, x.CreatedAt })
            .HasFilter("\"Status\" = 'Pending'"); // Partial index برای پردازش سریع‌تر
    }
}
