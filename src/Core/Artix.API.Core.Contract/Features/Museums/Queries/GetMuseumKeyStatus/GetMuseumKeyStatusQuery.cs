namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;

using Primitives.Handlers;

public sealed class GetMuseumKeyStatusQuery : IQuery<MuseumKeyStatusDto>
{
    public long MuseumId { get; init; }
    public long UserId { get; set; }
}
