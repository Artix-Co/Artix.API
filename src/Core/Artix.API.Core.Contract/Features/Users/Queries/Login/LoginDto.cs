namespace Artix.API.Core.Contract.Features.Users.Queries.Login;

public class LoginDto
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime AccessTokenExpiresAt { get; init; }
    public DateTime RefreshTokenExpiresAt { get; init; }
    public string Username { get; set; }
    public string? DisplayName { get; set; }
    public List<string> Roles { get; set; }
}
