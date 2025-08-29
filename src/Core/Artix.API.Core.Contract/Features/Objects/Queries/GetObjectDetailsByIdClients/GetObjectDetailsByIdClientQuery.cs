namespace Artix.API.Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdClients;

using Primitives.Handlers;

public sealed record GetObjectDetailsByIdClientQuery(Guid Id ) : IQuery<ObjectDetailsByIdClientDto>;
