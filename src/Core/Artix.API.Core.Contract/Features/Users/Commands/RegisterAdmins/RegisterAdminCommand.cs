namespace Artix.API.Core.Contract.Features.Users.Commands.RegisterAdmins;

using Primitives.Handlers;

public sealed class RegisterAdminCommand : ICommand
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
}
