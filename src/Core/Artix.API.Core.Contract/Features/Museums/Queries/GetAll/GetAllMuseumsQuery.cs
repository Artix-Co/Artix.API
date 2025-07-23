namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAll;

using Primitives.Handlers;

public sealed class GetAllMuseumsQuery : IQuery<IEnumerable<AllMuseumDto>>
{
    public string? Name { get; set; }
}
