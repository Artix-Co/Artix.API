namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetDetailByIds;

using Contract.Features.Caches.Objects;
using Contract.Features.Objects.Queries;
using Contract.Features.Objects.Queries.GetDetailByIds;
using Domain.Entities.User;
using Exceptions;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetObjectDetailByIdQueryHandler : QueryHandlerBase<GetObjectDetailByIdQuery, ObjectDetailByIdDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;
    private readonly ICacheService<RecentObjectDto> _objectCache;

    public GetObjectDetailByIdQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository,
        ICacheService<RecentObjectDto> objectCache) : base(cache, httpContextAccessor, userManager)
    {
        this._objectQueryRepository = objectQueryRepository;
        this._objectCache = objectCache;
    }

    public override async Task<ObjectDetailByIdDto> Handle(GetObjectDetailByIdQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);

        var result = await this._objectQueryRepository.GetDetailsByIdAsync(query, cancellationToken);

        if (result == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(result), query.Id);
        }

        await this._objectCache.AddToRecentAsync(user.Id.ToString(), RecentObjectDto.Create(result.BusinessId, result.Name));
        // await _museumCache.ClearRecentAsync(user.Id.ToString());
        return result;
    }
}
