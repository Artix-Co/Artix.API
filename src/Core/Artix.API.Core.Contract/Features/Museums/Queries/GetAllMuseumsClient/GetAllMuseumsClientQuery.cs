namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;

using Primitives.Handlers;

public sealed record GetAllMuseumsClientQuery(string? Name) : IQuery<IEnumerable<AllMuseumsClientDto>>;
