namespace Artix.API.Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdAdmins;

using Primitives.Handlers;

public record GetObjectDetailsByIdAdminQuery(Guid Id) : IQuery<ObjectDetailsByIdAdminDto>;
