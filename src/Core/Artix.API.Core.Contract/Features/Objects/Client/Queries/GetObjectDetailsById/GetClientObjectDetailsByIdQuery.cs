namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById;

using Primitives.Handlers;

public sealed record GetClientObjectDetailsByIdQuery(Guid Id ) : IQuery<ClientObjectDetailsByIdDto>;
