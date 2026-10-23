namespace Artix.API.Infra.Sql.Data.Config.Write.Audit;
using Core.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
internal sealed class AdminAuditEntryWriteConfiguration : IEntityTypeConfiguration<AdminAuditEntry>
{
    public void Configure(EntityTypeBuilder<AdminAuditEntry> entity)
    {
        entity.ToTable("AdminAuditEntries"); entity.HasKey(x => x.Id);
        entity.Property(x => x.Method).HasMaxLength(16).IsRequired(); entity.Property(x => x.Path).HasMaxLength(512).IsRequired();
        entity.Property(x => x.Roles).HasMaxLength(512); entity.Property(x => x.IpAddress).HasMaxLength(45); entity.Property(x => x.CreatedAt).IsRequired();
        entity.HasIndex(x => new { x.UserId, x.CreatedAt }); entity.HasIndex(x => x.Path);
    }
}
