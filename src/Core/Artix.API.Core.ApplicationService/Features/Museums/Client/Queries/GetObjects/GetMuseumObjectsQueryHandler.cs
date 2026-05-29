namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetObjects;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Museums;
using Contract.Features.Museums.Client.Queries.GetObjects;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler
internal sealed class
    GetMuseumObjectsQueryHandler : QueryHandlerBase<GetClientMuseumObjectsQuery, IEnumerable<ClientMuseumObjectDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetMuseumObjectsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<IEnumerable<ClientMuseumObjectDto>>> Handle(GetClientMuseumObjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = this._museumQueryRepository.GetObjects(query);
        return Result<IEnumerable<ClientMuseumObjectDto>>.Success(result);
    }
}
