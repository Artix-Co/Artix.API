namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseumsAdmin;

using Primitives.Models;

public sealed record GetAllMuseumsAdminQuery : PaginationQuery<AllMuseumsAdminDto>
{
    public bool? FilterByActive { get; set; }
}
