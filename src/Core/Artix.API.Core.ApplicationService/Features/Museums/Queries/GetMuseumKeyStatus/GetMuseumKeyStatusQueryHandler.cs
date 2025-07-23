namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetMuseumKeyStatus;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class GetMuseumKeyStatusQueryHandler : QueryHandlerBase<GetMuseumKeyStatusQuery, MuseumKeyStatusDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;

    public GetMuseumKeyStatusQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        IMuseumQueryRepository museumQueryRepository) : base(cache,
        httpContextAccessor)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<MuseumKeyStatusDto> Handle(GetMuseumKeyStatusQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetKeyStatusAsync(query, cancellationToken);
        
        if (result == null)
        {
            throw new KeyNotFoundException("The given museum key status could not be found.");
        }
        
        return result;
    }
}
