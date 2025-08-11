namespace Artix.API.Infra.Sql.Repositories.Features.Objects;

using Core.Contract.Features.Objects.Queries;
using Core.Contract.Features.Objects.Queries.GetDetailByIds;
using Core.Domain.Entities.Museum;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class ObjectQueryRepository : QueryRepository<Object>, IObjectQueryRepository
{
    private readonly ILogger<ObjectQueryRepository> _logger;
    private readonly ArtixQueryDbContext _queryDbContext;

    public ObjectQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<ObjectQueryRepository> logger)
        : base(queryDbContext)
    {
        _logger = logger;
        _queryDbContext = queryDbContext;
    }

    public async Task<ObjectDetailByIdDto> GetDetailsByIdAsync(GetObjectDetailByIdQuery dto,
        CancellationToken cancellationToken = default)
    {
        var query = await _queryDbContext.Objects
            .AsNoTracking()
            .Include(o => o.ObjectFiles)
            .ThenInclude(of => of.File)
            .Include(o => o.ObjectHistoricalPeriods)
            .ThenInclude(ohp => ohp.HistoricalPeriod)
            .AsSplitQuery()
            .FirstOrDefaultAsync(o => o.BusinessId == dto.Id, cancellationToken);

        if (query is null)
            throw InfrastructureNotFoundException.ForEntity(nameof(Object), dto.Id);


        if (query is null)
            throw InfrastructureNotFoundException.ForEntity(nameof(Object), dto.Id);

        var result = new ObjectDetailByIdDto
        {
            Id = query.Id,
            BusinessId = query.BusinessId,
            Name = query.Name,
            GeneralInformation = query.GeneralInformation,
            SpecializedInformation = query.SpecialInformation,
            HistoricalPeriods = query.ObjectHistoricalPeriods
                .Select(ohp => new HistoricalPeriodDto
                {
                    Id = ohp.HistoricalPeriod.BusinessId,
                    Name = ohp.HistoricalPeriod.Name,
                    Description = ohp.HistoricalPeriod.Description,
                    StartDate = ohp.HistoricalPeriod.StartDate,
                    EndDate = ohp.HistoricalPeriod.EndDate
                })
                .ToList(),
            Model3DBase64 = query.ObjectFiles
                .Where(of => of.File.MimeType == "model/obj" || of.File.MimeType == "model/gltf-binary")
                .Select(of => Convert.ToBase64String(System.IO.File.ReadAllBytes(of.File.FilePath)))
                .FirstOrDefault()
        };

        return result;
    }
}
