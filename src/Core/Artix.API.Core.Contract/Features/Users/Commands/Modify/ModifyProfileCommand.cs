namespace Artix.API.Core.Contract.Features.Users.Commands.Modify;

using Primitives.Handlers;

public sealed class ModifyProfileCommand: ICommand
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DisplayName { get; set; }
}
