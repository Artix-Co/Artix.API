namespace Artix.API.Core.Contract.Features.Users.Queries.Login;

public class LoginDto
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string? DisplayName { get; set; }
    public List<string> Roles { get; set; }
}
