namespace Artix.API.Core.Contract.Features.Users.Commands.RegisterMobiles;

using Primitives.Handlers;

public sealed class RegisterMobileCommand : ICommand<bool>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DisplayName { get; set; }
}
