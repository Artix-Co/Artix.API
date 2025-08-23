namespace Artix.API.Infra.Sql.Repositories.Features.Objects;

using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Features.Objects.Queries;
using Core.Contract.Features.Objects.Queries.GetDetailByIds;
using Core.Domain.Entities.Object;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class ObjectQueryRepository : QueryRepository<Object>, IObjectQueryRepository
{
    private readonly ILogger<ObjectQueryRepository> _logger;

    public ObjectQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<ObjectQueryRepository> logger)
        : base(queryDbContext)
    {
        _logger = logger;
    }

    public async Task<ObjectDetailByIdDto> GetDetailsByIdAsync(GetObjectDetailByIdQuery dto,
        CancellationToken cancellationToken = default)
    {
        var query = await _queryDbContext.Objects
            .Include(o => o.ObjectFiles)
            .ThenInclude(of => of.File)
            .Include(o => o.ObjectHistoricalPeriods)
            .ThenInclude(ohp => ohp.HistoricalPeriod)
            .AsSplitQuery()
            .FirstOrDefaultAsync(o => o.BusinessId == dto.Id, cancellationToken);

        if (query is null)
            throw InfrastructureNotFoundException.ForEntity(nameof(Object), dto.Id);


 

        var model3DBase64 = query.ObjectFiles
            .Where(of => of.File.MimeType is "model/obj" or "model/gltf-binary")
            .Select(of => Convert.ToBase64String(File.ReadAllBytes(of.File.FilePath)))
            .FirstOrDefault();

        var historicalPeriodsList = query.ObjectHistoricalPeriods
            .Select(ohp => new HistoricalPeriodDto
            (
                ohp.HistoricalPeriod.BusinessId,
                ohp.HistoricalPeriod.Name,
                ohp.HistoricalPeriod.Description,
                ohp.HistoricalPeriod.StartDate,
                ohp.HistoricalPeriod.EndDate
            ))
            .ToList();

        var result = new ObjectDetailByIdDto
        (
            query.BusinessId,
            query.Name,
            query.GeneralInformation,
            query.SpecialInformation,
            model3DBase64,
            historicalPeriodsList
        );

        return result;
    }
}
