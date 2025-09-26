namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetObjects;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetObjects;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetObjectsQueryHandler : QueryHandlerBase<GetAllObjectsQuery, PaginatedResult<AllObjectDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly IMemoryCache _cache;

    public GetObjectsQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(cache,
        httpContextAccessor, userManager)
    {
        _museumQueryRepository = museumQueryRepository;
        _cache = cache;
    }

    public override async Task<Result<PaginatedResult<AllObjectDto>>> Handle(GetAllObjectsQuery query,
        CancellationToken cancellationToken)
    {
        // Create a unique cache key based on all query parameters
        var cacheKey = $"Objects_All_{query.PageNumber}_{query.PageSize}_" +
                       $"{string.Join(",", query.CategoryIds?.OrderBy(id => id))}|" +
                       $"{query.NameFilter ?? ""}_{query.MuseumId?.ToString() ?? ""}_" +
                       $"{query.IsSpecial}_{query.IsHidden}_{query.Tier}_{query.Version}_" +
                       $"{query.SortBy}_{query.SortDescending}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out PaginatedResult<AllObjectDto>? cachedResult) && cachedResult != null)
        {
            this._httpContextAccessor.HttpContext!.Response.Headers["Cache-Control"] = "public, max-age=30";
            return Result<PaginatedResult<AllObjectDto>>.Success(cachedResult);
        }

        // Fetch from repository if not cached
        var result = await _museumQueryRepository.GetAllObjectsAsync(query, cancellationToken);

        // Cache the result
        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) // Align with NGINX TTL
        });

        this._httpContextAccessor.HttpContext!.Response.Headers["Cache-Control"] = "public, max-age=30";
        return Result<PaginatedResult<AllObjectDto>>.Success(result);
    }
}
