namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetObjects;

using Contract.Features.Museums;
using Contract.Features.Museums.Admin.Queries.GetPaginateObjects;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validator for this handler
internal sealed class
    GetMuseumObjectsQueryHandler : QueryHandlerBase<GetAdminMuseumObjectsQuery, PaginatedResult<AdminMuseumObjectDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetMuseumObjectsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<PaginatedResult<AdminMuseumObjectDto>>> Handle(GetAdminMuseumObjectsQuery query,
        CancellationToken cancellationToken)    
    {
        var result = await this._museumQueryRepository.GetAdminObjectsAsync(query,cancellationToken);
        return Result<PaginatedResult<AdminMuseumObjectDto>>.Success(result);
    }
}
