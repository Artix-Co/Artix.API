namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetObjectDetailsByIdClients;

using Contract.Features.Caches.Objects;
using Contract.Features.Objects.Queries;
using Contract.Features.Objects.Queries.GetObjectDetailsByIdClients;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Exceptions;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class
    GetObjectDetailsByIdClientQueryHandler : QueryHandlerBase<GetObjectDetailsByIdClientQuery,
    ObjectDetailsByIdClientDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;
    private readonly ICacheRepository<RecentObjectDto> _objectCache;


    public GetObjectDetailsByIdClientQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository, ICacheRepository<RecentObjectDto> objectCache) : base(httpContextAccessor, userManager)
    {
        this._objectQueryRepository = objectQueryRepository;
        this._objectCache = objectCache;
    }

    public override async Task<Result<ObjectDetailsByIdClientDto>> Handle(GetObjectDetailsByIdClientQuery clientQuery,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);

        var result = await this._objectQueryRepository.GetDetailsByIdAsync(clientQuery, cancellationToken);

        if (result == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(result), clientQuery.Id);
        }

        // await this._objectCache.SetAsync(user.Id.ToString(),
        //     RecentObjectDto.Create(result.Id, result.ImageUrl, result.Model3DUrl, result.Name,
        //         result.HistoricalPeriods));
        // await _museumCache.ClearRecentAsync(user.Id.ToString());


        return Result<ObjectDetailsByIdClientDto>.Success(result);
    }
}
