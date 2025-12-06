namespace Artix.API.Core.Contract.Features.Museums.Queries.GetDetailByIds;

using Primitives.Handlers;

public sealed record GetMuseumDetailsByIdQuery(Guid Id) : IQuery<MuseumDetailsByIdDto>;
