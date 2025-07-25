namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjectScans;

using Primitives.Handlers;

public sealed class GetObjectScanQuery : IQuery<ObjectScanDto>
{
    public long Id { get; set; }
}
