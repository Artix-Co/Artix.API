namespace Artix.API.Infra.Sql.Repositories.Features.Objects;

using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Features.Objects.Queries;
using Core.Contract.Features.Objects.Queries.GetAllObjectsAdmins;
using Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdAdmins;
using Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdClients;
using Core.Contract.Primitives.Models;
using Core.Domain.Entities.Object;
using Data.DbContexts;
using DPG.Core.Contract.Primitives.Models;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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


    public async Task<ObjectDetailsByIdClientDto> GetDetailsByIdAsync(
        GetObjectDetailsByIdClientQuery dto,
        CancellationToken cancellationToken = default)
    {
        var query = await _queryDbContext.Objects
            .Where(o => o.BusinessId == dto.Id)
            .Select(o => new
            {
                Object = new { o.BusinessId, o.Name, o.GeneralInformation, o.SpecialInformation },
                Model3D = o.Object3DModels
                    .Where(of => of.File.MimeType == "model/obj" || of.File.MimeType == "model/gltf-binary")
                    .Select(of => new { of.File.FilePath })
                    .FirstOrDefault(),
                Image = o.ObjectImages
                    .Where(of => of.File.MimeType == "jpg/png" || of.File.MimeType == "jpeg/webp")
                    .Select(of => new { of.File.FilePath })
                    .FirstOrDefault(),
                HistoricalPeriods = o.ObjectHistoricalPeriods
                    .Select(ohp => new HistoricalPeriodDto(
                        ohp.HistoricalPeriod.BusinessId,
                        ohp.HistoricalPeriod.Name,
                        ohp.HistoricalPeriod.Description,
                        ohp.HistoricalPeriod.StartDate,
                        ohp.HistoricalPeriod.EndDate
                    ))
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        if (query is null)
            throw InfrastructureNotFoundException.ForEntity(nameof(Object), dto.Id);

        string? model3DBase64 = null;
        if (query.Model3D?.FilePath != null)
        {
            await using var fileStream = new FileStream(
                query.Model3D.FilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true
            );
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, 8192, cancellationToken);
            model3DBase64 = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        string? imageBase64 = null;
        if (query.Image?.FilePath != null)
        {
            await using var fileStream = new FileStream(
                query.Image.FilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true
            );
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, 8192, cancellationToken);
            imageBase64 = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        var historicalPeriodsList = query.HistoricalPeriods.AsEnumerable().ToList();

        return new ObjectDetailsByIdClientDto(
            Id: query.Object.BusinessId,
            Name: query.Object.Name,
            GeneralInformation: query.Object.GeneralInformation,
            SpecialInformation: query.Object.SpecialInformation,
            Model3DBase64: model3DBase64,
            ImageBase64: imageBase64,
            HistoricalPeriods: historicalPeriodsList
        );
    }


    public async Task<PaginatedResult<AllObjectsAdminDto>> GetAllObjectsAdminAsync(
        GetAllObjectsAdminQuery dto,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(dto.PageNumber, 1);
        var pageSize = Math.Max(dto.PageSize, 1);

        var query = _queryDbContext.Objects
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.GlobalSearch))
        {
            var searchTerm = dto.GlobalSearch.ToLower();
            query = query.Where(o =>
                o.Name.ToLower().Contains(searchTerm) ||
                (o.GeneralInformation != null && o.GeneralInformation.ToLower().Contains(searchTerm)) ||
                (o.SpecialInformation != null && o.SpecialInformation.ToLower().Contains(searchTerm)) ||
                _queryDbContext.MuseumObjects.Any(mo => mo.ObjectId == o.Id &&
                                                        _queryDbContext.Museums.Any(m =>
                                                            m.Id == mo.MuseumId &&
                                                            m.Name.ToLower().Contains(searchTerm))));
        }

        if (!string.IsNullOrWhiteSpace(dto.SortBy))
        {
            query = dto.SortBy.ToLower() switch
            {
                "name" => dto.SortDirection == SortDirection.Asc
                    ? query.OrderBy(o => o.Name)
                    : query.OrderByDescending(o => o.Name),
                "version" => dto.SortDirection == SortDirection.Asc
                    ? query.OrderBy(o => o.Version)
                    : query.OrderByDescending(o => o.Version),
                _ => query.OrderBy(o => o.Name)
            };
        }
        else
        {
            query = query.OrderBy(o => o.Name);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pagedItems = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .GroupJoin(
                _queryDbContext.MuseumObjects,
                obj => obj.Id,
                mo => mo.ObjectId,
                (obj, museumObjects) => new { Object = obj, MuseumObjects = museumObjects }
            )
            .SelectMany(
                x => x.MuseumObjects.DefaultIfEmpty(),
                (obj, mo) => new { obj.Object, MuseumObject = mo }
            )
            .GroupJoin(
                _queryDbContext.Museums,
                x => x.MuseumObject != null ? x.MuseumObject.MuseumId : 0, // اصلاح 0 به Guid.Empty
                museum => museum.Id,
                (x, museums) => new { x.Object, Museums = museums }
            )
            .GroupBy(x => new
            {
                x.Object.BusinessId,
                x.Object.Name,
                x.Object.GeneralInformation,
                x.Object.SpecialInformation,
                x.Object.Version
            })
            .Select(g => new
            {
                g.Key.BusinessId,
                g.Key.Name,
                g.Key.GeneralInformation,
                g.Key.SpecialInformation,
                MuseumNames = g.SelectMany(m => m.Museums.Select(museum => museum.Name)).Distinct(),
                g.Key.Version
            })
            .ToListAsync(cancellationToken);

        // تبدیل به AllObjectsAdminDto در سمت کلاینت
        var resultItems = pagedItems.Select(item => new AllObjectsAdminDto(
            item.BusinessId,
            item.Name,
            item.GeneralInformation,
            item.SpecialInformation,
            string.Join(", ", item.MuseumNames.Where(name => !string.IsNullOrEmpty(name))),
            item.Version
        )).ToList();

        return new PaginatedResult<AllObjectsAdminDto>(
            Items: resultItems.AsReadOnly(),
            TotalCount: totalCount,
            PageNumber: pageNumber,
            PageSize: pageSize,
            Draw: true
        );
    }

    public async Task<ObjectDetailsByIdAdminDto> GetAllObjectDetailsByIdAdminAsync(
        GetObjectDetailsByIdAdminQuery dto,
        CancellationToken cancellationToken = default)
    {
        var query = await _queryDbContext.Objects
            .Where(o => o.BusinessId == dto.Id)
            .Select(o => new
            {
                Object = o,
                Model3D = o.Object3DModels
                    .Where(of => of.File.MimeType == "model/obj" || of.File.MimeType == "model/gltf-binary")
                    .Select(of => new { of.File.FilePath })
                    .FirstOrDefault(),
                Image = o.ObjectImages
                    .Where(of => of.File.MimeType == "jpg/png" || of.File.MimeType == "jpeg/webp")
                    .Select(of => new { of.File.FilePath })
                    .FirstOrDefault(),
                HistoricalPeriods = o.ObjectHistoricalPeriods
                    .Select(ohp => new HistoricalPeriodDto(
                        ohp.HistoricalPeriod.BusinessId,
                        ohp.HistoricalPeriod.Name,
                        ohp.HistoricalPeriod.Description,
                        ohp.HistoricalPeriod.StartDate,
                        ohp.HistoricalPeriod.EndDate
                    )),
                Types = o.ObjectTypes
                    .Select(ot => new TypeDto(
                        ot.Type.BusinessId,
                        ot.Type.Name,
                        ot.Type.Description
                    ))
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        if (query is null)
            throw InfrastructureNotFoundException.ForEntity(nameof(Object), dto.Id);

        string? model3DBase64 = null;
        if (query.Model3D?.FilePath != null)
        {
            await using var fileStream = new FileStream(query.Model3D.FilePath, FileMode.Open, FileAccess.Read,
                FileShare.Read, bufferSize: 4096, useAsync: true);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            model3DBase64 = Convert.ToBase64String(memoryStream.ToArray());
        }

        string? imageBase64 = null;
        if (query.Image?.FilePath != null)
        {
            await using var fileStream = new FileStream(query.Image.FilePath, FileMode.Open, FileAccess.Read,
                FileShare.Read, bufferSize: 4096, useAsync: true);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            imageBase64 = Convert.ToBase64String(memoryStream.ToArray());
        }

        var historicalPeriodsList = query.HistoricalPeriods.AsEnumerable().ToList();
        var typesList = query.Types.AsEnumerable().ToList();

        return new ObjectDetailsByIdAdminDto(
            Id: query.Object.BusinessId,
            Name: query.Object.Name,
            GeneralInformation: query.Object.GeneralInformation,
            SpecialInformation: query.Object.SpecialInformation,
            Version: query.Object.Version,
            Tier: query.Object.Tier,
            IsSpecial: query.Object.IsSpecial,
            IsHidden: query.Object.IsHidden,
            ObjectSaleType: query.Object.ObjectSaleType,
            CreatedAt: query.Object.CreatedAt,
            ImageBase64: imageBase64,
            Model3DBase64: model3DBase64,
            ObjectTypes: typesList,
            HistoricalPeriods: historicalPeriodsList
        );
    }
}
