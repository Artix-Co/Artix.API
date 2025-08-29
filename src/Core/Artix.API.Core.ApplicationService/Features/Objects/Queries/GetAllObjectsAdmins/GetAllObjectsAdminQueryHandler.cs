namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetAllObjectsAdmins;

using Contract.Features.Objects.Queries;
using Contract.Features.Objects.Queries.GetAllObjectsAdmins;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator
internal sealed class
    GetAllObjectsAdminQueryHandler : QueryHandlerBase<GetAllObjectsAdminQuery, PaginatedResult<AllObjectsAdminDto>>
{
    private readonly IObjectQueryRepository _objectQueryRepository;

    public GetAllObjectsAdminQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository) : base(cache,
        httpContextAccessor, userManager)
    {
        this._objectQueryRepository = objectQueryRepository;
    }

    public override async Task<Result<PaginatedResult<AllObjectsAdminDto>>> Handle(GetAllObjectsAdminQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._objectQueryRepository.GetAllObjectsAdminAsync(query, cancellationToken);
        return Result<PaginatedResult<AllObjectsAdminDto>>.Success(result);
    }
}
