namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetDetailByIds;

using Exceptions;
using Primitives;
using Artix.API.Core.Contract.Features.Caches.Museums;
using Artix.API.Core.Contract.Features.Museums.Commands;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

// TODO: develop validator for this handler
internal sealed class GetMuseumByIdQueryHandler : QueryHandlerBase<GetMuseumDetailsByIdQuery, MuseumDetailsByIdDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly ICacheService<RecentMuseumDto> _museumCache;

    public GetMuseumByIdQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository,
        ICacheService<RecentMuseumDto> museumCache) : base(cache,
        httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
        this._museumCache = museumCache;
    }

    public override async Task<Result<MuseumDetailsByIdDto>> Handle(GetMuseumDetailsByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);

        var result = await this._museumQueryRepository.GetDetailsByIdAsync(query, cancellationToken);

        if (result == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(result), query.Id);
        }

        await this._museumCache.AddToRecentAsync(user.Id.ToString(), RecentMuseumDto.Create(result.BusinessId, result.Name!));
        // await _museumCache.ClearRecentAsync(user.Id.ToString());
        return Result<MuseumDetailsByIdDto>.Success(result);
    }
}
