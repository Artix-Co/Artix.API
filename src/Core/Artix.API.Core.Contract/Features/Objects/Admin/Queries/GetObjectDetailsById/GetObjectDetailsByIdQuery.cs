namespace Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;

using Primitives.Handlers;

public sealed record GetObjectDetailsByIdQuery(Guid Id) : IQuery<ObjectDetailsByIdDto>;
