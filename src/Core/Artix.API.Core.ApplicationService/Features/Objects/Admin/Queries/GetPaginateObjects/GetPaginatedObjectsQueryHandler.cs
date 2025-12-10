namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Queries.GetPaginateObjects;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Objects;
using Contract.Features.Objects.Admin.Queries.GetPaginateObjects;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator
internal sealed class
    GetPaginatedObjectsQueryHandler : QueryHandlerBase<GetPaginateObjectsQuery, PaginatedResult<PaginateObjectsDto>>
{
    private readonly IObjectQueryRepository _objectQueryRepository;


    public GetPaginatedObjectsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._objectQueryRepository = objectQueryRepository;
    }

    public override async Task<Result<PaginatedResult<PaginateObjectsDto>>> Handle(GetPaginateObjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._objectQueryRepository.GetAllObjectsAdminAsync(query, cancellationToken);
        return Result<PaginatedResult<PaginateObjectsDto>>.Success(result);
    }
}
