namespace Artix.API.Infra.Sql.Repositories.Features.Museums;

using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;
using Artix.API.Core.Contract.Primitives.Models;
using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract.Features.Museums.Queries.GetAllMuseumsAdmin;
using Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;
using Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Data.CompiledQueries.Museums;
using Data.DbContexts;
using DPG.Core.Contract.Primitives.Models;
using Exceptions;
using Primitives;
using File = System.IO.File;

public sealed class MuseumQueryRepository : QueryRepository<Museum>, IMuseumQueryRepository
{
    private readonly ILogger<MuseumQueryRepository> _logger;

    public MuseumQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<MuseumQueryRepository> logger)
        : base(queryDbContext)
    {
        _logger = logger;
    }

    public IEnumerable<AllMuseumsClientDto> GetAllMuseumsClient(GetAllMuseumsClientQuery dto)
    {
        _logger.LogInformation("Fetching all museums with query: {@Query}", dto);

        var museums = this._queryDbContext.Museums
            .Include(o => o.MuseumImages)
            .ThenInclude(of => of.FileEntity)
            .AsSplitQuery()
            .OrderBy(m => m.Name)
            .AsEnumerable() 
            .Where(m => string.IsNullOrEmpty(dto.Name) || m.Name.Contains(dto.Name))
            .Select(m => new
            {
                Museum = m,
                ImagePath = m.MuseumImages
                    .Where(of => of.FileEntity.MimeType == "jpg" ||
                                 of.FileEntity.MimeType == "png" ||
                                 of.FileEntity.MimeType == "jpeg" ||
                                 of.FileEntity.MimeType == "webp")
                    .Select(of => of.FileEntity.FilePath)
                    .FirstOrDefault()
            })
            .Select(x => new AllMuseumsClientDto(
                x.Museum.BusinessId,
                x.Museum.Name,
                x.ImagePath != null ? TryReadFileAsBase64(x.ImagePath) : null, // File I/O moved to client-side
                x.Museum.Description,
                x.Museum.CreatedAt,
                x.Museum.IsActive
            ));
        
        if (museums == null || !museums.Any())
        {
            throw InfrastructureNotFoundException.WithMessage("No museums found!");
        }

        return museums;
    }

    public IEnumerable<MuseumObjectDto> GetObjects(GetMuseumObjectsQuery dto)
    {
        _logger.LogInformation("Fetching objects for museum ID: {MuseumId}", dto.MuseumId);

        var objects = MuseumQueries.GetMuseumObjectsQuery(_queryDbContext, dto.MuseumId);

        return objects;
    }

    public MuseumDetailsByIdDto GetDetailsById(GetMuseumDetailsByIdQuery dto)
    {
        _logger.LogInformation("Fetching museum with ID: {MuseumId}", dto.Id);

        var museum = MuseumQueries.GetDetailsByIdQuery(this._queryDbContext, dto.Id);

        if (museum == null)
        {
            throw InfrastructureNotFoundException.ForEntity(nameof(Museum), dto.Id);
        }

        return museum;
    }

    public async Task<PaginatedResult<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching objects with query: {@Query}", dto);

        var objects = MuseumQueries.GetAllObjectsQuery(
            _queryDbContext,
            dto.NameFilter,
            dto.PageNumber,
            dto.PageSize
        ).ToList().AsReadOnly();

        var totalCount = await _queryDbContext.Objects
            .Where(o => string.IsNullOrWhiteSpace(dto.NameFilter) || o.Name.Contains(dto.NameFilter))
            .CountAsync(cancellationToken);

        return new PaginatedResult<AllObjectDto>(
            objects,
            totalCount,
            dto.PageNumber,
            true,
            dto.PageSize
        );
    }


    public IEnumerable<MuseumJournalEntryDto> GetJournalEntries(GetMuseumJournalEntriesQuery dto)
    {
        _logger.LogInformation("Fetching journal entries for museum ID: {MuseumId}", dto.MuseumId);

        var journalEntries =
            (from m in _queryDbContext.Museums
                where m.Id == dto.MuseumId
                join mo in _queryDbContext.MuseumObjects on m.Id equals mo.MuseumId
                join o in _queryDbContext.Objects on mo.ObjectId equals o.Id
                join je in _queryDbContext.JournalEntries on o.Id equals je.ObjectId
                join uje in _queryDbContext.UserJournalEntries on je.Id equals uje.JournalEntryId into ujeGroup
                from uje in ujeGroup.DefaultIfEmpty()
                join u in _queryDbContext.Users on uje.UserId equals u.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                select new MuseumJournalEntryDto
                (
                    je.BusinessId,
                    m.BusinessId,
                    user.BusinessId,
                    je.Notes,
                    je.CreatedAt,
                    je.Title,
                    je.SketchUrl
                ))
            .AsEnumerable()
            .OrderByDescending(x => x.CreatedAt);


        return journalEntries;
    }

    public async Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching key status for museum ID: {MuseumId}, user ID: {UserId}", dto.MuseumId,
            dto.UserId);

        var museum = await _queryDbContext.Museums
            .FirstOrDefaultAsync(m => m.BusinessId == dto.MuseumId, cancellationToken);

        if (museum == null)
            return null;

        var keyStatus = await
            (from umk in _queryDbContext.UserMuseumKeys
                where umk.MuseumId == museum.Id && umk.UserId == dto.UserId
                join u in _queryDbContext.Users on umk.UserId equals u.Id
                select new MuseumKeyStatusDto(dto.MuseumId, true, umk.UnlockedAt, null))
            .FirstOrDefaultAsync(cancellationToken);


        if (keyStatus == null)
        {
            _logger.LogInformation("No key found for museum ID {MuseumId} and user ID {UserId}", dto.MuseumId,
                dto.UserId);
            throw InfrastructureNotFoundException.ForEntity(nameof(Museum), dto.MuseumId);
        }

        _logger.LogInformation("Successfully retrieved key status for museum ID {MuseumId}, user ID {UserId}",
            dto.MuseumId, dto.UserId);
        return keyStatus;
    }


    public async Task<PaginatedResult<AllMuseumsAdminDto>> GetAllMuseumsAdminAsync(
        GetAllMuseumsAdminQuery dto,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(dto.PageNumber, 1);
        var pageSize = Math.Max(dto.PageSize, 1);

        var query = _queryDbContext.Museums
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.GlobalSearch))
        {
            var searchTerm = dto.GlobalSearch.ToLower();
            query = query.Where(m =>
                m.Name.ToLower().Contains(searchTerm) ||
                m.Description.ToLower().Contains(searchTerm));
        }

        if (dto.FilterByActive.HasValue)
        {
            query = query.Where(m => m.IsActive == dto.FilterByActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(dto.SortBy))
        {
            query = dto.SortBy.ToLower() switch
            {
                "name" => dto.SortDirection == SortDirection.Asc
                    ? query.OrderBy(m => m.Name)
                    : query.OrderByDescending(m => m.Name),
                "createdat" => dto.SortDirection == SortDirection.Asc
                    ? query.OrderBy(m => m.CreatedAt)
                    : query.OrderByDescending(m => m.CreatedAt),
                "isactive" => dto.SortDirection == SortDirection.Asc
                    ? query.OrderBy(m => m.IsActive)
                    : query.OrderByDescending(m => m.IsActive),
                _ => query.OrderBy(m => m.CreatedAt)
            };
        }
        else
        {
            query = query.OrderBy(m => m.CreatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var pagedItems = await query
            .Select(m => new AllMuseumsAdminDto(
                m.BusinessId,
                m.Name,
                m.Description,
                m.CreatedAt,
                m.IsActive))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AllMuseumsAdminDto>(
            Items: pagedItems,
            TotalCount: totalCount,
            PageNumber: pageNumber,
            Draw: true,
            PageSize: pageSize
        );
    }

    private static string TryReadFileAsBase64(string filePath)
    {
        try
        {
            return Convert.ToBase64String(File.ReadAllBytes(filePath));
        }
        catch (Exception ex)
        {
            // Log the error (e.g., using ILogger)
            // _logger.LogError(ex, "Failed to read file: {FilePath}", filePath);
            return null;
        }
    }
}
