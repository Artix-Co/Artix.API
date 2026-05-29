namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetPaginateObjects;

using Primitives;
using Artix.API.Core.Contract.Features.Museums;
using Artix.API.Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler
internal sealed class GetPaginateObjectsQueryHandler : QueryHandlerBase<GetClientPaginateObjectsQuery, PaginatedResult<ClientPaginateObjectsDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetPaginateObjectsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<PaginatedResult<ClientPaginateObjectsDto>>> Handle(GetClientPaginateObjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllObjectsAsync(query, cancellationToken);


        return Result<PaginatedResult<ClientPaginateObjectsDto>>.Success(result);
    }
}
