namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseums;

using Artix.API.Core.Contract.Primitives.Handlers;

public sealed record GetAllMuseumsQuery(string? Name) : IQuery<IEnumerable<AllMuseumsDto>>;
