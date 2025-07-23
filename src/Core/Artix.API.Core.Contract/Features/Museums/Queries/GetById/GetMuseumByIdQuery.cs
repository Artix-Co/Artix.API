namespace Artix.API.Core.Contract.Features.Museums.Queries.GetById;

using Primitives.Handlers;

public sealed class GetMuseumByIdQuery : IQuery<MuseumByIdDto>
{
    public long Id { get; init; }
}
