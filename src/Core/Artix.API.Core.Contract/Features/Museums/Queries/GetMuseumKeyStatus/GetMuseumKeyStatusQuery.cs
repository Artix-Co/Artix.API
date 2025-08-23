namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;

using Primitives.Handlers;

public sealed record GetMuseumKeyStatusQuery(Guid MuseumId, long UserId) : IQuery<MuseumKeyStatusDto>;
