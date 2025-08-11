namespace Artix.API.Core.Contract.Features.Users.Commands.InitiateOTPAuth;

using Primitives.Handlers;

public sealed class InitiateOTPAuthCommand: ICommand
{
    public required string PhoneNumber { get; set; }
}
