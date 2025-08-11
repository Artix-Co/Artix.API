namespace Artix.API.Core.Contract.Features.Users.Queries.VerifyOTPAuth;

public sealed class VerifyOTPAuthDto
{
    public bool IsNewUser { get; set; }
    public Guid UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    
    // TODO: add user roles(user subscription)
}
