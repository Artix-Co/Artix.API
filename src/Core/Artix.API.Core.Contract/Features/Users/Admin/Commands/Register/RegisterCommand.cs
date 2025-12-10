namespace Artix.API.Core.Contract.Features.Users.Admin.Commands.Register;

using Primitives.Handlers;

public sealed record RegisterCommand(string Username, string Email, string Password, string DisplayName)
    : ICommand;
