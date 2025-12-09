namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetObjects;

using Primitives;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Museums.Client.Queries;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler
internal sealed class GetObjectsQueryHandler : QueryHandlerBase<GetAllObjectsQuery, PaginatedResult<AllObjectDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetObjectsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<PaginatedResult<AllObjectDto>>> Handle(GetAllObjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllObjectsAsync(query, cancellationToken);


        return Result<PaginatedResult<AllObjectDto>>.Success(result);
    }
}
