namespace Artix.API.Core.Domain.Entities.OTP;

using Common;
using Enums;
using Exceptions;

public class OTP : AggregateRoot
{
    public string PhoneNumber { get; private set; }
    public string Code { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public PurposeType Purpose { get; private set; }

    protected OTP()
    {
    }

    private OTP(string phoneNumber, string code, PurposeType purpose, int validityMinutes = 5)
    {
        PhoneNumber = phoneNumber;
        Code = code;
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
        var random = new Random();
        // return random.Next(100_000, 1_000_000).ToString()
        return "123456";
    }

    public bool IsValid(string code)
    {
        if (IsUsed)
            throw DomainException.InvalidOperation("OTP already used.");

        if (DateTime.UtcNow > ExpiresAt)
            throw DomainException.InvalidOperation("OTP has expired.");

        if (this.Code != code)
            throw DomainException.InvalidValue("OTP code");
        
        return true; 
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw DomainException.InvalidOperation("OTP is already marked as used.");

        IsUsed = true;
    }

    public void ExtendExpiry(int additionalMinutes)
    {
        ExpiresAt = ExpiresAt.AddMinutes(additionalMinutes);
    }
}
