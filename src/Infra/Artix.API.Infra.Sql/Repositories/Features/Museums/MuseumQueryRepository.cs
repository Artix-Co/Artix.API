


namespace Artix.API.Infra.Sql.Repositories.Features.Museums;

using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetAll;
using Artix.API.Core.Contract.Features.Museums.Queries.GetById;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;
using Artix.API.Core.Contract.Primitives.Models;
using Artix.API.Core.Domain.Entities.Museum;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.DbContexts;
using Primitives;

public sealed class MuseumQueryRepository : QueryRepository<Museum>, IMuseumQueryRepository
{
    private readonly ILogger<MuseumQueryRepository> _logger;
    private readonly ArtixQueryDbContext _queryDbContext;

    public MuseumQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<MuseumQueryRepository> logger)
        : base(queryDbContext)
    {
        _logger = logger;
        _queryDbContext = queryDbContext;
    }

    public async Task<IEnumerable<AllMuseumDto>> GetAllAsync(GetAllMuseumsQuery dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching all museums with query: {@Query}", dto);

            var museumsQuery = _queryDbContext.Museums
                .Where(m => !m.IsDeleted);

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                museumsQuery = museumsQuery.Where(m => m.Name.Contains(dto.Name));
            }

            var museums = await museumsQuery
                .Select(m => new AllMuseumDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    CreatedAt = m.CreatedAt,
                    IsActive = m.IsActive
                })
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

    public async Task<MuseumByIdDto?> GetDetailsByIdAsync(GetMuseumByIdQuery dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching museum with ID: {MuseumId}", dto.Id);

            var museum = await _queryDbContext.Museums
                .Where(m => m.Id == dto.Id && !m.IsDeleted)
                .GroupJoin(_queryDbContext.MuseumObjects,
                    m => m.Id,
                    mo => mo.MuseumId,
                    (m, moGroup) => new
                    {
                        Museum = m,
                        MuseumObjects = moGroup,
                        JournalEntryCount = _queryDbContext.JournalEntries
                            .Count(je => !je.IsDeleted && moGroup.Any(mo => mo.ObjectId == je.ObjectId))
                    })
                .Select(x => new MuseumByIdDto
                {
                    Id = x.Museum.Id,
                    Name = x.Museum.Name,
                    Description = x.Museum.Description,
                    CreatedAt = x.Museum.CreatedAt,
                    ModifiedAt = x.Museum.ModifiedAt,
                    IsActive = x.Museum.IsActive,
                    ObjectCount = x.MuseumObjects.Count(),
                    JournalEntryCount = x.JournalEntryCount
                })
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

    public async Task<IEnumerable<MuseumObjectDto>> GetObjectsAsync(GetMuseumObjectsQuery dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching objects for museum ID: {MuseumId}", dto.MuseumId);

            var objects = await _queryDbContext.MuseumObjects
                .Where(mo => mo.MuseumId == dto.MuseumId && !mo.IsDeleted)
                .Join(_queryDbContext.Objects,
                    mo => mo.ObjectId,
                    o => o.Id,
                    (mo, o) => new MuseumObjectDto
                    {
                        Id = o.Id, // Use Object.Id instead of MuseumObject.Id
                        MuseumId = mo.MuseumId,
                        Name = o.Name,
                        Description = o.GeneralInformation, // Map GeneralInformation to Description
                        CreatedAt = o.CreatedAt
                    })
                .OrderBy(mo => mo.Name)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} objects for museum ID {MuseumId}", objects.Count, dto.MuseumId);
            return objects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching objects for museum ID {MuseumId}", dto.MuseumId);
            throw;
        }
    }

    public async Task<IEnumerable<MuseumJournalEntryDto>> GetJournalEntriesAsync(GetMuseumJournalEntriesQuery dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching journal entries for museum ID: {MuseumId}", dto.MuseumId);

            var journalEntries = await _queryDbContext.Museums
                .Where(m => m.Id == dto.MuseumId && !m.IsDeleted)
                .Join(_queryDbContext.MuseumObjects,
                    m => m.Id,
                    mo => mo.MuseumId,
                    (m, mo) => new { Museum = m, MuseumObject = mo })
                .Where(x => !x.MuseumObject.IsDeleted)
                .Join(_queryDbContext.Objects,
                    x => x.MuseumObject.ObjectId,
                    o => o.Id,
                    (x, o) => new { x.Museum, Object = o })
                .Join(_queryDbContext.JournalEntries,
                    x => x.Object.Id,
                    je => je.ObjectId,
                    (x, je) => new MuseumJournalEntryDto
                    {
                        Id = je.Id,
                        MuseumId = x.Museum.Id,
                        UserId = je.UserJournalEntries.Any() ? je.UserJournalEntries.First().UserId : 0,
                        Content = je.Notes,
                        CreatedAt = je.CreatedAt,
                        Title = je.Title,
                        SketchUrl = je.SketchUrl
                    })
                .OrderByDescending(dto => dto.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} journal entries for museum ID {MuseumId}", journalEntries.Count, dto.MuseumId);
            return journalEntries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching journal entries for museum ID {MuseumId}", dto.MuseumId);
            throw;
        }
    }

    public async Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching key status for museum ID: {MuseumId}, user ID: {UserId}", dto.MuseumId, dto.UserId);

            var keyStatus = await _queryDbContext.UserMuseumKeys
                .Where(umk => umk.MuseumId == dto.MuseumId && umk.UserId == dto.UserId && !umk.IsDeleted)
                .Select(umk => new MuseumKeyStatusDto
                {
                    MuseumId = umk.MuseumId,
                    UserId = umk.UserId,
                    HasKey = true,
                    GrantedAt = umk.AcquiredAt,
                    ExpiresAt = null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (keyStatus == null)
            {
                _logger.LogInformation("No key found for museum ID {MuseumId} and user ID {UserId}", dto.MuseumId, dto.UserId);
                return new MuseumKeyStatusDto
                {
                    MuseumId = dto.MuseumId,
                    UserId = dto.UserId,
                    HasKey = false,
                    GrantedAt = null,
                    ExpiresAt = null
                };
            }

            _logger.LogInformation("Successfully retrieved key status for museum ID {MuseumId}, user ID {UserId}", dto.MuseumId, dto.UserId);
            return keyStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching key status for museum ID {MuseumId}, user ID {UserId}", dto.MuseumId, dto.UserId);
            throw;
        }
    }

    public async Task<PagedData<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching objects with query: {@Query}", dto);

            var objectsQuery = _queryDbContext.Objects
                .Where(o => !o.IsDeleted)
                .Join(_queryDbContext.MuseumObjects,
                    o => o.Id,
                    mo => mo.ObjectId,
                    (o, mo) => new { Object = o, MuseumObject = mo })
                .Where(x => !x.MuseumObject.IsDeleted);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(dto.NameFilter))
            {
                objectsQuery = objectsQuery.Where(x => x.Object.Name.Contains(dto.NameFilter));
            }

            if (dto.MuseumId.HasValue)
            {
                objectsQuery = objectsQuery.Where(x => x.MuseumObject.MuseumId == dto.MuseumId.Value);
            }

            if (dto.CategoryIds.Any())
            {
                objectsQuery = objectsQuery.Where(x => _queryDbContext.ObjectTypes
                    .Any(moc => moc.ObjectId == x.Object.Id && dto.CategoryIds.Contains(moc.TypeId)));
            }

            if (dto.IsSpecial.HasValue)
            {
                objectsQuery = objectsQuery.Where(x => x.Object.IsSpecial == dto.IsSpecial.Value);
            }

            if (dto.IsHidden.HasValue)
            {
                objectsQuery = objectsQuery.Where(x => x.Object.IsHidden == dto.IsHidden.Value);
            }

            if (dto.Tier.HasValue)
            {
                objectsQuery = objectsQuery.Where(x => x.Object.Tier == dto.Tier.Value);
            }

            if (dto.Version.HasValue)
            {
                objectsQuery = objectsQuery.Where(x => x.Object.Version == dto.Version.Value);
            }

            // Get total count before pagination
            var totalCount = await objectsQuery.CountAsync(cancellationToken);

            // Apply sorting
            objectsQuery = dto.SortBy?.ToLowerInvariant() switch
            {
                "createdat" => dto.SortDescending
                    ? objectsQuery.OrderByDescending(x => x.Object.CreatedAt)
                    : objectsQuery.OrderBy(x => x.Object.CreatedAt),
                "tier" => dto.SortDescending
                    ? objectsQuery.OrderByDescending(x => x.Object.Tier ?? int.MaxValue)
                    : objectsQuery.OrderBy(x => x.Object.Tier ?? int.MaxValue),
                _ => dto.SortDescending
                    ? objectsQuery.OrderByDescending(x => x.Object.Name)
                    : objectsQuery.OrderBy(x => x.Object.Name)
            };

            // Apply pagination
            objectsQuery = objectsQuery
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize);

            // Project to DTO with category names
            var objects = await objectsQuery
                .GroupJoin(_queryDbContext.ObjectTypes,
                    x => x.Object.Id,
                    moc => moc.ObjectId,
                    (x, mocGroup) => new { x.Object, MuseumObjectCategories = mocGroup })
                .SelectMany(
                    x => x.MuseumObjectCategories.DefaultIfEmpty(),
                    (x, moc) => new { x.Object, MuseumObjectCategory = moc })
                .GroupJoin(_queryDbContext.Types.Where(c => !c.IsDeleted),
                    x => x.MuseumObjectCategory != null ? x.MuseumObjectCategory.TypeId : 0,
                    c => c.Id,
                    (x, cGroup) => new { x.Object, CategoryNames = cGroup.Select(c => c.Name).ToList() })
                .GroupBy(x => x.Object)
                .Select(g => new AllObjectDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Description = g.Key.GeneralInformation,
                    MuseumId = _queryDbContext.MuseumObjects
                        .Where(mo => mo.ObjectId == g.Key.Id)
                        .Select(mo => mo.MuseumId)
                        .FirstOrDefault(),
                    QRCode = g.Key.QrCode,
                    IsSpecial = g.Key.IsSpecial,
                    IsHidden = g.Key.IsHidden,
                    Tier = g.Key.Tier,
                    Version = g.Key.Version,
                    CreatedAt = g.Key.CreatedAt,
                    CategoryNames = g.SelectMany(x => x.CategoryNames).Distinct().ToList().AsReadOnly()
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} objects for query", objects.Count);

            return new PagedData<AllObjectDto>(
                items: objects,
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
