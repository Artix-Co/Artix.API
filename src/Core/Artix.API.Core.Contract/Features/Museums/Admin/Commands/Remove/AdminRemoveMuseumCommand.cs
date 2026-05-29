namespace Artix.API.Core.Contract.Features.Museums.Admin.Commands.Remove;

using Primitives.Handlers;

public sealed record AdminRemoveMuseumCommand(Guid Id) : ICommand;
