namespace Artix.API.Core.Domain.Entities.OTP;

using Common;

public class OTP : AggregateRoot
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
        // TODO: uncomment to auto generate randomly on prod (Generate 6-digit OTP)
        //var code = new Random().Next(100000, 999999).ToString(); 

        var code = "123456";

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
        return !this.IsUsed && DateTime.UtcNow <= this.ExpiresAt && this.Code == code;
    }
}
