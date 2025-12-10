namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetAll;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Museums;
using Contract.Features.Museums.Client.Queries.GetAll;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler
internal sealed class GetAllMuseumsQueryHandler : QueryHandlerBase<GetAllMuseumsQuery, IEnumerable<AllMuseumsDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetAllMuseumsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<IEnumerable<AllMuseumsDto>>> Handle(GetAllMuseumsQuery query,
        CancellationToken cancellationToken)
    {
        var result = this._museumQueryRepository.GetAllMuseumsClient(query);
        return Result<IEnumerable<AllMuseumsDto>>.Success(result);
    }
}
