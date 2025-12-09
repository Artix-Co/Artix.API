namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetKeyStatus;

using Primitives.Handlers;

public sealed record GetMuseumKeyStatusQuery(Guid MuseumId, long UserId) : IQuery<MuseumKeyStatusDto>;
