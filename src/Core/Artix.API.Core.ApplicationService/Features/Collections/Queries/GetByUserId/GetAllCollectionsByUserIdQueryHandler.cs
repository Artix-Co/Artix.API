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
    GetAllCollectionsByUserIdQueryHandler : QueryHandlerBase<GetCollectionsByUserIdQuery, IEnumerable<CollectionsByUserIdDto>>
{
    private readonly ICollectionQueryRepository _collectionQueryRepository;

    public GetAllCollectionsByUserIdQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, ICollectionQueryRepository collectionQueryRepository) : base(cache,
        httpContextAccessor, userManager)
    {
        this._collectionQueryRepository = collectionQueryRepository;
    }

    public override async Task<Result<IEnumerable<CollectionsByUserIdDto>>> Handle(GetCollectionsByUserIdQuery query, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var userCollectionList =
            this._collectionQueryRepository.GetCollectionsByUserId(new GetUserCollectionsQuery { UserId = user.Id, });

        var result = userCollectionList.Select(uc => new CollectionsByUserIdDto
        {
            Id = uc.Id, Name = uc.Name, Description = uc.Description,
        });
        return Result<IEnumerable<CollectionsByUserIdDto>>.Success(result);
    }
}
