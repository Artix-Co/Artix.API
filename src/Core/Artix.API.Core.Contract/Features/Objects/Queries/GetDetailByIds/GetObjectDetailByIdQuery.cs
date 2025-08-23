namespace Artix.API.Core.Contract.Features.Objects.Queries.GetDetailByIds;

using Primitives.Handlers;

public sealed record GetObjectDetailByIdQuery(Guid Id ) : IQuery<ObjectDetailByIdDto>;
