namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetById;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetById;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetMuseumByIdQueryHandler : QueryHandlerBase<GetMuseumByIdQuery, MuseumByIdDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetMuseumByIdQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(cache, httpContextAccessor, userManager)
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
