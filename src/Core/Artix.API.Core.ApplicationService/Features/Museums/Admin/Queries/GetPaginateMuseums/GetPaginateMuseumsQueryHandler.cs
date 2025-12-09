namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetPaginateMuseums;

using Primitives;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Museums;
using Contract.Features.Museums.Admin.Queries.GetPaginate;
using Contract.Features.Museums.Client.Queries;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class
    GetPaginateMuseumsQueryHandler : QueryHandlerBase<GetPaginateMuseumsQuery, PaginatedResult<PaginatedMuseumsDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;

    public GetPaginateMuseumsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<PaginatedResult<PaginatedMuseumsDto>>> Handle(GetPaginateMuseumsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllMuseumsAdminAsync(query, cancellationToken);
        return Result<PaginatedResult<PaginatedMuseumsDto>>.Success(result);
    }
}
