namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetAllMuseumsClient;

using Primitives;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

// TODO: develop validator for this handler
internal sealed class GetAllMuseumsClientQueryHandler : QueryHandlerBase<GetAllMuseumsClientQuery, IEnumerable<AllMuseumsClientDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetAllMuseumsClientQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<IEnumerable<AllMuseumsClientDto>>> Handle(GetAllMuseumsClientQuery clientQuery,
        CancellationToken cancellationToken)
    {
        var result = this._museumQueryRepository.GetAllMuseumsClient(clientQuery);
        return Result<IEnumerable<AllMuseumsClientDto>>.Success(result);
    }
}
