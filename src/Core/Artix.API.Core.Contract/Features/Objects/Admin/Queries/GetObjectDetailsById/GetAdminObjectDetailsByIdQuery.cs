namespace Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;

using Primitives.Handlers;

public sealed record GetAdminObjectDetailsByIdQuery(Guid Id) : IQuery<AdminObjectDetailsByIdDto>;
