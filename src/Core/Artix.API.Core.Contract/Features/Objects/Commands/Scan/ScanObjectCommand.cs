namespace Artix.API.Core.Contract.Features.Objects.Commands.Scan;

using Primitives.Handlers;

public sealed record ScanObjectCommand(Guid ObjectId, Guid MuseumId) : ICommand;
