namespace Artix.API.Core.Contract.Primitives.Models;

using Handlers;

public abstract record PagedQuery<TResponse>(
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedData<TResponse>>;
