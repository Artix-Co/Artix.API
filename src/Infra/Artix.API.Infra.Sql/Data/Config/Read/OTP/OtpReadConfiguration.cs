namespace Artix.API.Infra.Sql.Data.Config.Read.OTP;

using Core.Domain.Entities.OTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class OtpReadConfiguration : BaseEntityConfiguration<OTP>
{
    public override void Configure(EntityTypeBuilder<OTP> builder)
    {
        base.Configure(builder);

        builder.ToTable("OTPs");
        
        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(6);
        
        
        builder.Property(o => o.ExpiresAt)
            .IsRequired();
        
        builder.Property(o => o.IsUsed)
            .IsRequired();
        
        
        builder.Property(o => o.Purpose)
            .IsRequired()
            .HasMaxLength(50);
        
        
        builder.HasIndex(o => o.PhoneNumber)
            .HasDatabaseName("IX_OTP_PhoneNumber");

        builder.HasIndex(o => o.Code)
            .HasDatabaseName("IX_OTP_Code");

        builder.HasIndex(o => o.ExpiresAt)
            .HasDatabaseName("IX_OTP_ExpiresAt");

        builder.HasIndex(o => new { o.PhoneNumber, o.Code })
            .HasDatabaseName("IX_OTP_PhoneNumber_Code");

        builder.HasIndex(o => new { o.PhoneNumber, o.ExpiresAt })
            .HasDatabaseName("IX_OTP_PhoneNumber_ExpiresAt");
    }
}
