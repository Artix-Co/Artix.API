namespace Artix.API.Core.Domain.Entities.User;

using Common;

public class OTP : BaseEntity
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // 6-digit OTP
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public string Purpose { get; set; } = string.Empty; // "Registration" or "Login"

    protected OTP()
    {
    }

    public static OTP Create(string phoneNumber, string purpose, int validityMinutes = 5)
    {
        var code = new Random().Next(100000, 999999).ToString(); // Generate 6-digit OTP
        return new OTP
        {
            PhoneNumber = phoneNumber,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(validityMinutes),
            IsUsed = false,
            Purpose = purpose
        };
    }

    public bool IsValid(string code)
    {
        return !IsUsed && DateTime.UtcNow <= ExpiresAt && Code == code;
    }
}
