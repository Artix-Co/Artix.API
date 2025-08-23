namespace Artix.API.Core.Contract.Features.Users.Commands.Modify;

using Primitives.Handlers;

public sealed record ModifyProfileCommand(string? Username,string? Password,string? Email,string? PhoneNumber,string? DisplayName): ICommand;
