namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetAll;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetAll;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetAllMuseumQueryHandler : QueryHandlerBase<GetAllMuseumsQuery, IEnumerable<AllMuseumDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetAllMuseumQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(cache, httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<IEnumerable<AllMuseumDto>>> Handle(GetAllMuseumsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetAllAsync(query, cancellationToken);
        return Result<IEnumerable<AllMuseumDto>>.Success(result);
    }
}
