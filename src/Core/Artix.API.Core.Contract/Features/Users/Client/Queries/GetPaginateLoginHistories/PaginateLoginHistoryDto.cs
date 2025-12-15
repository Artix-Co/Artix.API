namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetPaginateLoginHistories;

public sealed record PaginateLoginHistoryDto()
{
    public string UserAgent { get; set; }
    public string IpAddress { get; set; }
    public DateTime Date { get; set; }
    public bool IsActive { get; set; }
}
