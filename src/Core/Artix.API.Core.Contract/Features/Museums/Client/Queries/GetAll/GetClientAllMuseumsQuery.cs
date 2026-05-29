namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetAll;

using Primitives.Handlers;

public sealed record GetClientAllMuseumsQuery(string? Name) : IQuery<IEnumerable<ClientAllMuseumsDto>>;
