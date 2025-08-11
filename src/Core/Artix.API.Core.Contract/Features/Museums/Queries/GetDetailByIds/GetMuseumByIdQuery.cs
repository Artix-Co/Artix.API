namespace Artix.API.Core.Contract.Features.Museums.Queries.GetDetailByIds;

using Primitives.Handlers;

public sealed class GetMuseumByIdQuery : IQuery<MuseumByIdDto>
{
    public Guid Id { get; set; }
}
