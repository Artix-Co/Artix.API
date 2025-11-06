namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetMuseumKeyStatus;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetMuseumKeyStatusQueryHandler : QueryHandlerBase<GetMuseumKeyStatusQuery, MuseumKeyStatusDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetMuseumKeyStatusQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<MuseumKeyStatusDto>> Handle(GetMuseumKeyStatusQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetKeyStatusAsync(query, cancellationToken);

        if (result == null)
        {
            // TODO: convert it to ApplicationServiceNotFoundException.ForEntity
            throw new KeyNotFoundException("The given museum key status could not be found.");
        }

        return Result<MuseumKeyStatusDto>.Success(result);
    }
}
