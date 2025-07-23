namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetAll;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetAll;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class GetAllMuseumQueryHandler : QueryHandlerBase<GetAllMuseumsQuery, IEnumerable<AllMuseumDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;

    public GetAllMuseumQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        IMuseumQueryRepository museumQueryRepository) : base(cache,
        httpContextAccessor)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<IEnumerable<AllMuseumDto>> Handle(GetAllMuseumsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllAsync(query, cancellationToken);
        return result;
    }
}
