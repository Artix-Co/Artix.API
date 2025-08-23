namespace Artix.API.Core.Contract.Features.Versions.Queries.GetLast;

using Primitives.Handlers;

public sealed record GetLastVersionQuery : IQuery<LastVersionDto>;
