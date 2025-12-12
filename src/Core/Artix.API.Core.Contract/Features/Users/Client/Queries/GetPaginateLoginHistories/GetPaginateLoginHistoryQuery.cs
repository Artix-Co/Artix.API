namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetPaginateLoginHistories;

using Primitives.Models;

public sealed record GetPaginateLoginHistoryQuery: PaginationQuery<PaginateLoginHistoryDto>;
