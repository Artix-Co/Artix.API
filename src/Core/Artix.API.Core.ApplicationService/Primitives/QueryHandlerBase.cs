namespace Artix.API.Core.ApplicationService.Primitives;

using Contract.Primitives.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

public abstract class QueryHandlerBase<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    protected readonly IMemoryCache Cache;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected QueryHandlerBase(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
    {
        this.Cache = cache;
        this._httpContextAccessor = httpContextAccessor;
    }

    public abstract Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
