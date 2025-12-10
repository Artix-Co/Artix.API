namespace Artix.API.Core.Contract.Features.Objects.Client.Commands.Scan;

using Primitives.Handlers;

public sealed record ScanObjectCommand(Guid ObjectId, Guid MuseumId) : ICommand;
