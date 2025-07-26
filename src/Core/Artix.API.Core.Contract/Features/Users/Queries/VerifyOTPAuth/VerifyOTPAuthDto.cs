namespace Artix.API.Core.Contract.Features.Users.Queries.VerifyOTPAuth;

public sealed class VerifyOTPAuthDto
{
    public bool IsNewUser { get; set; }
    public long UserId { get; set; }
    public string? Token { get; set; }
}
