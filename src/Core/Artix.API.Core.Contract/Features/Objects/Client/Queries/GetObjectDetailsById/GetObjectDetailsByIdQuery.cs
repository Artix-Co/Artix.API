namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById;

using Primitives.Handlers;

public sealed record GetObjectDetailsByIdQuery(Guid Id ) : IQuery<ObjectDetailsByIdDto>;
