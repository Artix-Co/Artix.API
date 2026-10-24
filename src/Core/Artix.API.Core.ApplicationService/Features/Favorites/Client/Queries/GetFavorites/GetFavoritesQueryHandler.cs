namespace Artix.API.Core.ApplicationService.Features.Favorites.Client.Queries.GetFavorites;

using Contract.Features.Favorites;
using Contract.Features.Favorites.Client.Queries.GetFavorites;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;
using Contract.Primitives.Models;

internal sealed class GetFavoritesQueryHandler : QueryHandlerBase<GetFavoritesQuery, IReadOnlyList<FavoriteDto>>
{
    private readonly IFavoriteQueryRepository _favoriteRepository;

    public GetFavoritesQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IFavoriteQueryRepository favoriteRepository) : base(httpContextAccessor, userManager)
    {
        _favoriteRepository = favoriteRepository;
    }

    public override async Task<Result<IReadOnlyList<FavoriteDto>>> Handle(GetFavoritesQuery query,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        return Result<IReadOnlyList<FavoriteDto>>.Success(
            await _favoriteRepository.GetByUserIdAsync(user.Id, cancellationToken));
    }
}
