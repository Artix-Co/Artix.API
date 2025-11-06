namespace Artix.API.Core.ApplicationService.Features.Collections.Queries.GetByUserId;

using Contract.Features.Collections.Queries;
using Contract.Features.Collections.Queries.GetCollectionByUserId;
using Contract.Features.Collections.Queries.GetUserCollection;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class
    GetAllCollectionsByUserIdQueryHandler : QueryHandlerBase<GetCollectionsByUserIdQuery,
    IEnumerable<CollectionsByUserIdDto>>
{
    private readonly ICollectionQueryRepository _collectionQueryRepository;


    public GetAllCollectionsByUserIdQueryHandler(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, ICollectionQueryRepository collectionQueryRepository) : base(
        httpContextAccessor, userManager)
    {
        this._collectionQueryRepository = collectionQueryRepository;
    }

    public override async Task<Result<IEnumerable<CollectionsByUserIdDto>>> Handle(GetCollectionsByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var userCollectionList =
            this._collectionQueryRepository.GetCollectionsByUserId(new GetUserCollectionsQuery(user.Id));

        var result =
            userCollectionList.Select(uc => new CollectionsByUserIdDto(uc.Id, uc.Name, uc.Description, uc.IsPublic));
        return Result<IEnumerable<CollectionsByUserIdDto>>.Success(result);
    }
}
