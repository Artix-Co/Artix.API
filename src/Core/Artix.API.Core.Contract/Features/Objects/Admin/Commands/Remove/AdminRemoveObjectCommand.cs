namespace Artix.API.Core.Contract.Features.Objects.Admin.Commands.Remove;

using Primitives.Handlers;

public sealed record AdminRemoveObjectCommand(Guid Id) : ICommand;
