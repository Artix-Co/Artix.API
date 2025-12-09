namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Queries.GetAll;

using Primitives;
using Artix.API.Core.Contract.Features.Objects.Queries;
using Artix.API.Core.Contract.Features.Objects.Queries.GetAllObjectsAdmins;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator
internal sealed class
    GetAllObjectsQueryHandler : QueryHandlerBase<GetAllObjectsAdminQuery, PaginatedResult<AllObjectsAdminDto>>
{
    private readonly IObjectQueryRepository _objectQueryRepository;


    public GetAllObjectsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository) : base(httpContextAccessor, userManager)
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
