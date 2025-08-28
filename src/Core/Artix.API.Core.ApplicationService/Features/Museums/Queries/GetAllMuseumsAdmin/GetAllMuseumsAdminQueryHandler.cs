namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetAllMuseumsAdmin;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetAllMuseumsAdmin;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class
    GetAllMuseumsAdminQueryHandler : QueryHandlerBase<GetAllMuseumsAdminQuery, PaginatedResult<AllMuseumsAdminDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    public GetAllMuseumsAdminQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(cache, httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<PaginatedResult<AllMuseumsAdminDto>>> Handle(GetAllMuseumsAdminQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllMuseumsAdminAsync(query, cancellationToken);
        return Result<PaginatedResult<AllMuseumsAdminDto>>.Success(result);
    }
}
