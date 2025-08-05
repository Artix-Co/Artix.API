namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetObjects;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetObjects;
using Contract.Primitives.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetObjectsQueryHandler : QueryHandlerBase<GetAllObjectsQuery, PagedData<AllObjectDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;

    public GetObjectsQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        IMuseumQueryRepository museumQueryRepository) : base(cache,
        httpContextAccessor)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<PagedData<AllObjectDto>> Handle(GetAllObjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllObjectsAsync(query, cancellationToken);
        return result;
    }
}
