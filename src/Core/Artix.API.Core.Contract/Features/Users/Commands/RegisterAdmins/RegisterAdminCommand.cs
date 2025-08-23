namespace Artix.API.Core.Contract.Features.Users.Commands.RegisterAdmins;

using Primitives.Handlers;

public sealed record RegisterAdminCommand(string Username, string Email, string Password, string DisplayName)
    : ICommand;
