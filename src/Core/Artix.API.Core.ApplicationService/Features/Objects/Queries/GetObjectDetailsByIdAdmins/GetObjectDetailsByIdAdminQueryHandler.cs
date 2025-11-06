namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetObjectDetailsByIdAdmins;

using Contract.Features.Objects.Queries;
using Contract.Features.Objects.Queries.GetObjectDetailsByIdAdmins;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class
    GetObjectDetailsByIdAdminQueryHandler : QueryHandlerBase<GetObjectDetailsByIdAdminQuery, ObjectDetailsByIdAdminDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;


    public GetObjectDetailsByIdAdminQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._objectQueryRepository = objectQueryRepository;
    }

    public override async Task<Result<ObjectDetailsByIdAdminDto>> Handle(GetObjectDetailsByIdAdminQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._objectQueryRepository.GetObjectDetailsByIdAdminAsync(query, cancellationToken);
        return Result<ObjectDetailsByIdAdminDto>.Success(result);
    }
}
