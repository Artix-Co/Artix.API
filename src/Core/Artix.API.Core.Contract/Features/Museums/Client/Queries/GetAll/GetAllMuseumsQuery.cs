namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetAll;

using Primitives.Handlers;

public sealed record GetAllMuseumsQuery(string? Name) : IQuery<IEnumerable<AllMuseumsDto>>;
