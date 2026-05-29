namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetDetailByIds;

using Primitives.Handlers;

public sealed record GetClientMuseumDetailsByIdQuery(Guid Id) : IQuery<ClientMuseumDetailsByIdDto>;
