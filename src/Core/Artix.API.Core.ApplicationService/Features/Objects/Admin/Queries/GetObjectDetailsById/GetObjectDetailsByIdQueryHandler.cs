namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Queries.GetObjectDetailsById;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Objects;
using Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class
    GetObjectDetailsByIdQueryHandler : QueryHandlerBase<GetAdminObjectDetailsByIdQuery, AdminObjectDetailsByIdDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;


    public GetObjectDetailsByIdQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IObjectQueryRepository objectQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._objectQueryRepository = objectQueryRepository;
    }

    public override async Task<Result<AdminObjectDetailsByIdDto>> Handle(GetAdminObjectDetailsByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._objectQueryRepository.GetObjectDetailsByIdAdminAsync(query, cancellationToken);
        return Result<AdminObjectDetailsByIdDto>.Success(result);
    }
}
