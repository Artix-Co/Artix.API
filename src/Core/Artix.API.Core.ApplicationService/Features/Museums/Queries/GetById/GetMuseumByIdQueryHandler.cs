namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetById;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetById;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetMuseumByIdQueryHandler : QueryHandlerBase<GetMuseumByIdQuery, MuseumByIdDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;

    public GetMuseumByIdQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        IMuseumQueryRepository museumQueryRepository) : base(cache,
        httpContextAccessor)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<MuseumByIdDto> Handle(GetMuseumByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _museumQueryRepository.GetDetailsByIdAsync(query, cancellationToken);

        if (result == null)
        {
            // TODO: convert it to ApplicationServiceNotFoundException.ForEntity
            throw new KeyNotFoundException("The given museum could not be found.");
        }
        return result;
    }
}
