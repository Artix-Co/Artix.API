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

        var query = _queryDbContext.Objects.AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.GlobalSearch))
        {
            var searchTerm = dto.GlobalSearch.ToLower();
            query = query.Where(o =>
                o.Name.ToLower().Contains(searchTerm) ||
                (o.GeneralInformation != null && o.GeneralInformation.ToLower().Contains(searchTerm)) ||
                (o.SpecialInformation != null && o.SpecialInformation.ToLower().Contains(searchTerm)));
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

        query = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var pagedItems = await query
            .Select(o => new AllObjectsAdminDto(
                o.BusinessId,
                o.Name,
                o.GeneralInformation,
                o.SpecialInformation,
                o.Version
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AllObjectsAdminDto>(
            Items: pagedItems.AsReadOnly(),
            TotalCount: totalCount,
            PageNumber: pageNumber,
            Draw: true,
            PageSize: pageSize
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
