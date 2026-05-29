namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetPaginateMuseums;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Museums;
using Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class
    GetPaginateMuseumsQueryHandler : QueryHandlerBase<GetAdminPaginateMuseumsQuery, PaginatedResult<AdminPaginatedMuseumsDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;

    public GetPaginateMuseumsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<PaginatedResult<AdminPaginatedMuseumsDto>>> Handle(GetAdminPaginateMuseumsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllMuseumsAdminAsync(query, cancellationToken);
        return Result<PaginatedResult<AdminPaginatedMuseumsDto>>.Success(result);
    }
}
