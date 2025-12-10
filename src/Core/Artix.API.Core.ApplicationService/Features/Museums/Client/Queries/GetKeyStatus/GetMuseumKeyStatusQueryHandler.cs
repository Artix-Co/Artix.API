namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetKeyStatus;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Museums;
using Contract.Features.Museums.Client.Queries.GetKeyStatus;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
