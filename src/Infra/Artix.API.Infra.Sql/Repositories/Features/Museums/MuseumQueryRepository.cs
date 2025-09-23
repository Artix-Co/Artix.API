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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract.Features.Museums.Queries.GetAllMuseumsAdmin;
using Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;
using Core.Contract.Features.Museums.Queries.GetDetailByIds;
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

        var query = _queryDbContext.Museums
            .Include(o => o.MuseumImages)
            .ThenInclude(of => of.FileEntity)
            .AsSplitQuery();

        var imageBase64 = query
            .SelectMany(m => m.MuseumImages)
            .Where(of => of.FileEntity.MimeType == "jpg" || of.FileEntity.MimeType == "png" ||
                         of.FileEntity.MimeType == "jpeg" ||
                         of.FileEntity.MimeType == "webp")
            .Select(of => Convert.ToBase64String(File.ReadAllBytes(of.FileEntity.FilePath)))
            .FirstOrDefault();


        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            query = query.Where(m => m.Name.Contains(dto.Name));
        }


        var museums = query
            .AsEnumerable()
            .Select(m =>
                new AllMuseumsClientDto(m.BusinessId, m.Name, imageBase64, m.Description, m.CreatedAt, m.IsActive))
            .OrderBy(m => m.Name);

        if (museums == null)
        {
            throw InfrastructureNotFoundException.WithMessage("No museums found!");
        }

        return museums;
    }


    public async Task<MuseumDetailsByIdDto> GetDetailsByIdAsync(GetMuseumDetailsByIdQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching museum with ID: {MuseumId}", dto.Id);

        var museum = await _queryDbContext.Museums
            .Where(m => m.BusinessId == dto.Id)
            .GroupJoin(_queryDbContext.MuseumObjects,
                m => m.Id,
                mo => mo.MuseumId,
                (m, moGroup) => new
                {
                    Museum = m,
                    MuseumObjects = moGroup,
                    JournalEntryCount = _queryDbContext.JournalEntries
                        .Count(je => moGroup.Any(mo => mo.ObjectId == je.ObjectId))
                })
            .Select(x => new MuseumDetailsByIdDto(x.Museum.BusinessId, x.Museum.Name, x.Museum.Description,
                x.Museum.CreatedAt, x.Museum.IsActive, x.MuseumObjects.Count(), x.JournalEntryCount))
            .FirstOrDefaultAsync(cancellationToken);

        if (museum == null)
        {
            throw InfrastructureNotFoundException.ForEntity(nameof(Museum), dto.Id);
        }


        return museum;
    }

    public IEnumerable<MuseumObjectDto> GetObjects(GetMuseumObjectsQuery dto)
    {
        _logger.LogInformation("Fetching objects for museum ID: {MuseumId}", dto.MuseumId);

        var objects = _queryDbContext.MuseumObjects
            .AsEnumerable()
            .Join(
                _queryDbContext.Objects,
                mo => mo.ObjectId,
                o => o.Id,
                (mo, o) => new { MuseumObject = mo, Object = o })
            .Join(
                _queryDbContext.Museums,
                x => x.MuseumObject.MuseumId,
                m => m.Id,
                (x, m) => new { x.Object, x.MuseumObject, Museum = m })
            .Where(x => x.Museum.BusinessId == dto.MuseumId)
            .Select(x => new MuseumObjectDto
            (
                x.Object.BusinessId,
                x.Museum.BusinessId,
                x.Object.Name,
                x.Object.GeneralInformation,
                x.Object.CreatedAt
            ));

        if (objects is null)
        {
            throw InfrastructureNotFoundException.WithMessage("Museum objects not found!");
        }

        return objects;
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


    public async Task<PaginatedResult<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching objects with query: {@Query}", dto);

        var query = _queryDbContext.Objects.AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.NameFilter))
            query = query.Where(o => o.Name.Contains(dto.NameFilter));

        if (dto.MuseumId.HasValue)
            query = from o in query
                join mo in _queryDbContext.MuseumObjects on o.Id equals mo.ObjectId
                join m in _queryDbContext.Museums on mo.MuseumId equals m.Id
                where m.BusinessId == dto.MuseumId.Value
                select o;

        if (dto.CategoryIds != null)
            query = query.Where(o => _queryDbContext.ObjectTypes
                .Any(ot => ot.ObjectId == o.Id && dto.CategoryIds.Contains(ot.TypeId)));

        if (dto.IsSpecial.HasValue)
            query = query.Where(o => o.IsSpecial == dto.IsSpecial.Value);

        if (dto.IsHidden.HasValue)
            query = query.Where(o => o.IsHidden == dto.IsHidden.Value);

        if (dto.Tier.HasValue)
            query = query.Where(o => o.Tier == dto.Tier.Value);

        if (dto.Version.HasValue)
            query = query.Where(o => o.Version == dto.Version.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        query = dto.SortBy?.ToLowerInvariant() switch
        {
            "createdat" => dto.SortDescending
                ? query.OrderByDescending(o => o.CreatedAt)
                : query.OrderBy(o => o.CreatedAt),
            "tier" => dto.SortDescending
                ? query.OrderByDescending(o => o.Tier ?? int.MaxValue)
                : query.OrderBy(o => o.Tier ?? int.MaxValue),
            _ => dto.SortDescending ? query.OrderByDescending(o => o.Name) : query.OrderBy(o => o.Name)
        };

        var pagedObjects = query
            .Skip((dto.PageNumber - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .Select(o => new AllObjectDto
            (
                o.BusinessId,
                o.Name,
                o.GeneralInformation,
                (from mo in _queryDbContext.MuseumObjects
                    join m in _queryDbContext.Museums on mo.MuseumId equals m.Id
                    where mo.ObjectId == o.Id
                    select m.BusinessId).FirstOrDefault(),
                o.QrCode,
                o.IsSpecial,
                o.IsHidden,
                o.Tier,
                o.Version,
                o.CreatedAt,
                _queryDbContext.ObjectTypes
                    .Where(ot => ot.ObjectId == o.Id)
                    .Join(_queryDbContext.Types,
                        ot => ot.TypeId,
                        t => t.Id,
                        (ot, t) => new TypeDto(t.BusinessId, t.Name, t.Description))
                    .ToList(),
                _queryDbContext.HistoricalPeriods
                    .Where(hp => _queryDbContext.ObjectHistoricalPeriods
                        .Any(ohp => ohp.ObjectId == o.Id && ohp.HistoricalPeriodId == hp.Id))
                    .Select(hp => new HistoricalPeriodDto
                    (
                        hp.BusinessId,
                        hp.Name,
                        hp.Description,
                        hp.StartDate,
                        hp.EndDate
                    )).ToList()
            ))
            .ToList();

        return new PaginatedResult<AllObjectDto>(
            pagedObjects,
            totalCount,
            dto.PageNumber,
            true,
            dto.PageSize
        );
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
}
