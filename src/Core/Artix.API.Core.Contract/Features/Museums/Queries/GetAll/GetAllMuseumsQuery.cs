namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAll;

using Primitives.Handlers;
using Primitives.Models;

public sealed record GetAllMuseumsQuery(string? Name) : IQuery<IEnumerable<AllMuseumDto>>;
