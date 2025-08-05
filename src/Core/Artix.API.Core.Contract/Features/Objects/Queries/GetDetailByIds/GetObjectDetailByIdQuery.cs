namespace Artix.API.Core.Contract.Features.Objects.Queries.GetDetailByIds;

using Primitives.Handlers;

public sealed class GetObjectDetailByIdQuery : IQuery<ObjectDetailByIdDto>
{
    public long Id { get; set; }
}
