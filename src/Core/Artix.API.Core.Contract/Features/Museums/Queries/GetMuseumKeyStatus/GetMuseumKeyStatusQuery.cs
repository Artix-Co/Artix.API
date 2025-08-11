namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;

using Primitives.Handlers;

public sealed class GetMuseumKeyStatusQuery : IQuery<MuseumKeyStatusDto>
{
    public Guid MuseumId { get; set; }
    public long UserId { get; set; }
}
