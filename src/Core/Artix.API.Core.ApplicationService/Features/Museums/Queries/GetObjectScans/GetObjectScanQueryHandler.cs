namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetObjectScans;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetObjectScans;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class GetObjectScanQueryHandler : QueryHandlerBase<GetObjectScanQuery, ObjectScanDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    
    public GetObjectScanQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, IMuseumQueryRepository museumQueryRepository) : base(cache,
        httpContextAccessor)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<ObjectScanDto> Handle(GetObjectScanQuery query, CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetObjectScanAsync(query,cancellationToken);
        return result;
    }
}
