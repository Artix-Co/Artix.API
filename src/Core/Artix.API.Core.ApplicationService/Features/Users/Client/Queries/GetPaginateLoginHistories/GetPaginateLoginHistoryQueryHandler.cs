namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetPaginateLoginHistories;

using Contract.Features.Users.Client.Queries.GetPaginateLoginHistories;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;

internal sealed class GetPaginateLoginHistoryQueryHandler : QueryHandlerBase<GetPaginateLoginHistoryQuery,
    PaginatedResult<PaginateLoginHistoryDto>>
{
    private readonly ILogger<GetPaginateLoginHistoryQueryHandler> _logger;

    public GetPaginateLoginHistoryQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<GetPaginateLoginHistoryQueryHandler> logger)
        : base(httpContextAccessor, userManager)
    {
        _logger = logger;
    }

    public override async Task<Result<PaginatedResult<PaginateLoginHistoryDto>>> Handle(
        GetPaginateLoginHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);

        _logger.LogInformation(
            "Handling GetPaginateLoginHistoryQuery for UserId={UserId} Page={Page} Size={Size}",
            user.Id, query.PageNumber, query.PageSize);

        var totalCount = user.UserSessions.Count;

        _logger.LogInformation(
            "Retrieved user {UserId} with {LoginCount} login history records.",
            user.Id, totalCount);

        var pagedHistories = user.UserSessions
            .OrderByDescending(ulh => ulh.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ulh =>
                new PaginateLoginHistoryDto
                {
                    IpAddress = ulh.IpAddress,
                    UserAgent = ulh.UserAgent,
                    Date = ulh.CreatedAt,
                    IsActive = ulh.IsActive
                })
            .ToList()
            .AsReadOnly();

        _logger.LogInformation(
            "Returning {PagedCount} records for page {Page}.",
            pagedHistories.Count, query.PageNumber);

        var result = new PaginatedResult<PaginateLoginHistoryDto>(
            Items: pagedHistories,
            TotalCount: totalCount,
            PageNumber: query.PageNumber,
            PageSize: query.PageSize,
            Draw: true
        );

        return Result<PaginatedResult<PaginateLoginHistoryDto>>.Success(result);
    }
}
