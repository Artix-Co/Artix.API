namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Queries.GetObjectDetailsById;

using Primitives;
using Artix.API.Core.Contract.Features.Objects.Queries;
using Artix.API.Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdAdmins;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class
    GetObjectDetailsByIdQueryHandler : QueryHandlerBase<GetObjectDetailsByIdAdminQuery, ObjectDetailsByIdAdminDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;


    public GetObjectDetailsByIdQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository) : base(httpContextAccessor, userManager)
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
