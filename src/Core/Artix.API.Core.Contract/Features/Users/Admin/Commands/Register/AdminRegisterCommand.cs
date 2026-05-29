namespace Artix.API.Core.Contract.Features.Users.Admin.Commands.Register;

using Primitives.Handlers;

public sealed record AdminRegisterCommand(string Username, string Email, string Password, string DisplayName)
    : ICommand;
