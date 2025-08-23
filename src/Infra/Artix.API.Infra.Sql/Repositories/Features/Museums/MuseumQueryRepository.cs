namespace Artix.API.Infra.Sql.Repositories.Features.Museums;

using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetAll;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;
using Artix.API.Core.Contract.Primitives.Models;
using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Data.DbContexts;
using Primitives;

public sealed class MuseumQueryRepository : QueryRepository<Museum>, IMuseumQueryRepository
{
    private readonly ILogger<MuseumQueryRepository> _logger;

    public MuseumQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<MuseumQueryRepository> logger)
        : base(queryDbContext)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<AllMuseumDto>> GetAllAsync(GetAllMuseumsQuery dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching all museums with query: {@Query}", dto);

            var museumsQuery = _queryDbContext.Museums.AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                museumsQuery = museumsQuery.Where(m => m.Name.Contains(dto.Name));
            }

            var museums = await museumsQuery
                .Select(m => new AllMuseumDto(m.BusinessId, m.Name, m.Description, m.CreatedAt, m.IsActive))
                .OrderBy(m => m.Name)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} museums", museums.Count);
            return museums;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all museums");
            throw;
        }
    }

    public async Task<MuseumDetailsByIdDto?> GetDetailsByIdAsync(GetMuseumDetailsByIdQuery dto,
        CancellationToken cancellationToken = default)
    {
        try
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
                _logger.LogWarning("Museum with ID {MuseumId} not found", dto.Id);
            }
            else
            {
                _logger.LogInformation("Successfully retrieved museum with ID {MuseumId}", dto.Id);
            }

            return museum;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching museum with ID {MuseumId}", dto.Id);
            throw;
        }
    }

    public async Task<IEnumerable<MuseumObjectDto>> GetObjectsAsync(GetMuseumObjectsQuery dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching objects for museum ID: {MuseumId}", dto.MuseumId);

            var objects = await _queryDbContext.MuseumObjects
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
                ))
                .OrderBy(dto => dto.Name)
                .ToListAsync(cancellationToken);


            _logger.LogInformation("Successfully retrieved {Count} objects for museum ID {MuseumId}", objects.Count,
                dto.MuseumId);
            return objects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching objects for museum ID {MuseumId}", dto.MuseumId);
            throw;
        }
    }

    public async Task<IEnumerable<MuseumJournalEntryDto>> GetJournalEntriesAsync(GetMuseumJournalEntriesQuery dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching journal entries for museum ID: {MuseumId}", dto.MuseumId);

            var journalEntries = await
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
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully retrieved {Count} journal entries for museum ID {MuseumId}",
                journalEntries.Count,
                dto.MuseumId
            );

            return journalEntries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching journal entries for museum ID {MuseumId}",
                dto.MuseumId);
            throw;
        }
    }

    public async Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto,
        CancellationToken cancellationToken = default)
    {
        try
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
                    select new MuseumKeyStatusDto(dto.MuseumId, true, umk.AcquiredAt, null))
                .FirstOrDefaultAsync(cancellationToken);


            if (keyStatus == null)
            {
                _logger.LogInformation("No key found for museum ID {MuseumId} and user ID {UserId}", dto.MuseumId,
                    dto.UserId);
                return new MuseumKeyStatusDto(dto.MuseumId, false, null, null);
            }

            _logger.LogInformation("Successfully retrieved key status for museum ID {MuseumId}, user ID {UserId}",
                dto.MuseumId, dto.UserId);
            return keyStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching key status for museum ID {MuseumId}, user ID {UserId}",
                dto.MuseumId, dto.UserId);
            throw;
        }
    }


    public async Task<PagedData<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching objects with query: {@Query}", dto);

            var query = _queryDbContext.Objects
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.NameFilter))
                query = query.Where(o => o.Name.Contains(dto.NameFilter));

            if (dto.MuseumId.HasValue)
                query = from o in query
                    join mo in _queryDbContext.MuseumObjects on o.Id equals mo.ObjectId
                    join m in _queryDbContext.Museums on mo.MuseumId equals m.Id
                    where m.BusinessId == dto.MuseumId.Value
                    select o;

            if (dto.CategoryIds.Any())
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

            var pagedObjects = await query
                .Skip((dto.Page - 1) * dto.PageSize)
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
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} objects for query", pagedObjects.Count);

            return new PagedData<AllObjectDto>(
                items: pagedObjects,
                totalCount: totalCount,
                pageSize: dto.PageSize,
                currentPage: dto.Page
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching objects for query");
            throw;
        }
    }
}
