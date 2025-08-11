namespace Artix.API.Core.Contract.Features.Users.Queries.Login;

public sealed class LoginDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public string Username { get; set; }
    public string? DisplayName { get; set; }
    public List<string> Roles { get; set; }
}
