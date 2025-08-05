namespace Artix.API.Core.Contract.Features.Objects.Commands.ScanObject;

using Primitives.Handlers;

public class ScanObjectCommand : ICommand<long>
{
    public long ObjectId { get; set; }
    public long MuseumId { get; set; }
}
