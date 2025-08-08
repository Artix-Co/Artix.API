namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetById;

using Contract.Features.Caches.Museums;
using Contract.Features.Museums.Commands;
using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetById;
using Domain.Entities.Museum;
using Domain.Entities.User;
using Exceptions;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetMuseumByIdQueryHandler : QueryHandlerBase<GetMuseumByIdQuery, MuseumByIdDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly ICacheService<RecentMuseumDto> _museumCache;

    public GetMuseumByIdQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository,
        ICacheService<RecentMuseumDto> museumCache, IMuseumCommandRepository museumCommandRepository) : base(cache,
        httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
        this._museumCache = museumCache;
        this._museumCommandRepository = museumCommandRepository;
    }

    public override async Task<MuseumByIdDto> Handle(GetMuseumByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        var result = await _museumQueryRepository.GetDetailsByIdAsync(query, cancellationToken);

        if (result == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(result), query.Id);
        }


        var museum = await this._museumCommandRepository.GetByIdAsync(result.Id, cancellationToken);
        if (museum == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), result.Id);
        }

        await _museumCache.AddToRecentAsync(user.Id.ToString(), RecentMuseumDto.Create(museum.Id, museum.Name));
        return result;
    }
}
