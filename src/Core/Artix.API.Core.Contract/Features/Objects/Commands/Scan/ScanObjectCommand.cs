namespace Artix.API.Core.Contract.Features.Objects.Commands.Scan;

using Primitives.Handlers;

public sealed class ScanObjectCommand : ICommand<long>
{
    public long ObjectId { get; set; }
    public long MuseumId { get; set; }
}
