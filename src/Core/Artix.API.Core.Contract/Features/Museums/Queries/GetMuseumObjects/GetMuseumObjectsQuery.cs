namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumObjects;

using Primitives.Handlers;

public sealed class GetMuseumObjectsQuery : IQuery<IEnumerable<MuseumObjectDto>>
{
    public Guid MuseumId { get; set; }
}
