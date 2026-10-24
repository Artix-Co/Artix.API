using System.Security.Cryptography;

namespace Artix.API.Core.Domain.Entities.OTP;

using Common;
using Enums;
using Exceptions;

public class OTP : AggregateRoot
{
    public string PhoneNumber { get; private set; }
    public string Code { get; private set; }
    public string? CodeForDelivery { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public PurposeType Purpose { get; private set; }

    protected OTP()
    {
    }

    private OTP(string phoneNumber, string code, PurposeType purpose, int validityMinutes = 5)
    {
        PhoneNumber = phoneNumber;
        Code = Hash(code);
        CodeForDelivery = code;
        ExpiresAt = DateTime.UtcNow.AddMinutes(validityMinutes);
        IsUsed = false;
        Purpose = purpose;
    }

    public static OTP Generate(string phoneNumber, PurposeType purpose, int validityMinutes = 5)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw DomainException.InvalidValue(nameof(phoneNumber));

        var code = GenerateOtpCode();
        return new OTP(phoneNumber, code, purpose, validityMinutes);
    }

    private static string GenerateOtpCode()
    {
        return RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();
    }

    public bool IsValid(string code)
    {
        if (IsUsed)
            throw DomainException.InvalidOperation("OTP already used.");

        if (DateTime.UtcNow > ExpiresAt)
            throw DomainException.InvalidOperation("OTP has expired.");

        if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(this.Code), SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(code))))
            throw DomainException.InvalidValue("OTP code");
        
        return true; 
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw DomainException.InvalidOperation("OTP is already marked as used.");

        IsUsed = true;
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value)));

    public void ExtendExpiry(int additionalMinutes)
    {
        ExpiresAt = ExpiresAt.AddMinutes(additionalMinutes);
    }
}
