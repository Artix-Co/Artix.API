namespace Artix.API.Core.Contract.Features.Objects.Client.Commands.Scan;

using Primitives.Handlers;

public sealed record ClientScanObjectCommand(Guid ObjectId, Guid MuseumId) : ICommand;
