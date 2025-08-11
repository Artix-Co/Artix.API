namespace Artix.API.Core.Contract.Features.Objects.Commands.Scan;

using Primitives.Handlers;

public sealed class ScanObjectCommand: ICommand
{
    public Guid ObjectId { get; set; }
    public Guid MuseumId { get; set; }
}
