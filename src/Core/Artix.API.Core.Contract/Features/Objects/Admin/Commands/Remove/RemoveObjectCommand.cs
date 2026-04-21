namespace Artix.API.Core.Contract.Features.Objects.Admin.Commands.Remove;

using Primitives.Handlers;

public sealed record RemoveObjectCommand(Guid Id) : ICommand;
