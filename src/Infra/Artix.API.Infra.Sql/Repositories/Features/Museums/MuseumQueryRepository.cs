namespace Artix.API.Infra.Sql.Repositories.Features.Museums;

using Core.Contract.Features.Museums.Queries;
using Core.Contract.Features.Museums.Queries.GetAll;
using Core.Contract.Features.Museums.Queries.GetById;
using Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Primitives.Models;
using Core.Domain.Entities.Museum;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

    public async Task<IEnumerable<AllMuseumDto>> GetAllAsync(GetAllMuseumsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching all museums with query: {@Query}", query);

            var museumsQuery = _queryDbContext.Museums
                .Where(m => !m.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                museumsQuery = museumsQuery.Where(m => m.Name.Contains(query.Name));
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

    
    public async Task<MuseumByIdDto?> GetDetailsByIdAsync(GetMuseumByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching museum with ID: {MuseumId}", query.Id);

            var museum = await _queryDbContext.Museums
                .AsNoTracking()
                .Where(m => m.Id == query.Id && !m.IsDeleted)
                .GroupJoin(_queryDbContext.MuseumObjects.Where(mo => !mo.IsDeleted),
                    m => m.Id,
                    mo => mo.MuseumId,
                    (m, moGroup) => new 
                    { 
                        Museum = m, 
                        MuseumObjects = moGroup,
                        JournalEntryCount = this._queryDbContext.JournalEntries
                            .Count(je => !je.IsDeleted && moGroup.Select(mo => mo.Id).Contains(je.ObjectId))
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
                _logger.LogWarning("Museum with ID {MuseumId} not found", query.Id);
            }
            else
            {
                _logger.LogInformation("Successfully retrieved museum with ID {MuseumId}", query.Id);
            }

            return museum;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching museum with ID {MuseumId}", query.Id);
            throw;
        }
    }
    
    
    public async Task<IEnumerable<MuseumObjectDto>> GetObjectsAsync(GetMuseumObjectsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching objects for museum ID: {MuseumId}", query.MuseumId);

            var objects = await _queryDbContext.MuseumObjects
                .Where(mo => mo.MuseumId == query.MuseumId && !mo.IsDeleted)
                .Select(mo => new MuseumObjectDto
                {
                    Id = mo.Id,
                    MuseumId = mo.MuseumId,
                    Name = mo.Name,
                    Description = mo.Description,
                    CreatedAt = mo.CreatedAt
                })
                .OrderBy(mo => mo.Name)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} objects for museum ID {MuseumId}", objects.Count, query.MuseumId);
            return objects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching objects for museum ID {MuseumId}", query.MuseumId);
            throw;
        }
    }


    public async Task<IEnumerable<MuseumJournalEntryDto>> GetJournalEntriesAsync(GetMuseumJournalEntriesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching journal entries for museum ID: {MuseumId}", query.MuseumId);

            var journalEntries = await _queryDbContext.Museums
                .Where(m => m.Id == query.MuseumId && !m.IsDeleted)
                .Join(_queryDbContext.MuseumObjects,
                    m => m.Id,
                    mo => mo.MuseumId,
                    (m, mo) => new { Museum = m, MuseumObject = mo })
                .Where(x => !x.MuseumObject.IsDeleted)
                .Join(_queryDbContext.JournalEntries,
                    x => x.MuseumObject.Id,
                    je => je.ObjectId,
                    (x, je) => new MuseumJournalEntryDto
                    {
                        Id = je.Id,
                        MuseumId = x.Museum.Id,
                        UserId = je.UserJournalEntries.Any() ? je.UserJournalEntries.First().UserId : 0, // Adjust based on actual relationship
                        Content = je.Notes, // Assuming Notes is the content field
                        CreatedAt = je.CreatedAt,
                        Title = je.Title,
                        SketchUrl = je.SketchUrl
                    })

                .OrderByDescending(dto => dto.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} journal entries for museum ID {MuseumId}", journalEntries.Count, query.MuseumId);
            return journalEntries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching journal entries for museum ID {MuseumId}", query.MuseumId);
            throw;
        }
    }

   
    public async Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching key status for museum ID: {MuseumId}, user ID: {UserId}", query.MuseumId, query.UserId);

            var keyStatus = await _queryDbContext.UserMuseumKeys
                .AsNoTracking()
                .Where(umk => umk.MuseumId == query.MuseumId && umk.UserId == query.UserId && !umk.IsDeleted)
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
                _logger.LogInformation("No key found for museum ID {MuseumId} and user ID {UserId}", query.MuseumId, query.UserId);
                return new MuseumKeyStatusDto
                {
                    MuseumId = query.MuseumId,
                    UserId = query.UserId,
                    HasKey = false,
                    GrantedAt = null,
                    ExpiresAt = null
                };
            }

            _logger.LogInformation("Successfully retrieved key status for museum ID {MuseumId}, user ID {UserId}", query.MuseumId, query.UserId);
            return keyStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching key status for museum ID {MuseumId}, user ID {UserId}", query.MuseumId, query.UserId);
            throw;
        }
    }

    public async Task<PagedData<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery query, CancellationToken cancellationToken = default)
{
    try
    {
        _logger.LogInformation("Fetching museum objects with query: {@Query}", query);

        var objectsQuery = _queryDbContext.MuseumObjects
            .AsNoTracking()
            .Where(mo => !mo.IsDeleted);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.NameFilter))
        {
            objectsQuery = objectsQuery.Where(mo => mo.Name.Contains(query.NameFilter));
        }

        if (query.MuseumId.HasValue)
        {
            objectsQuery = objectsQuery.Where(mo => mo.MuseumId == query.MuseumId.Value);
        }

        if (query.CategoryIds.Any())
        {
            objectsQuery = objectsQuery.Where(mo => mo.MuseumObjectCategories
                .Any(moc => query.CategoryIds.Contains(moc.CategoryId)));
        }

        if (query.IsSpecial.HasValue)
        {
            objectsQuery = objectsQuery.Where(mo => mo.IsSpecial == query.IsSpecial.Value);
        }

        if (query.IsHidden.HasValue)
        {
            objectsQuery = objectsQuery.Where(mo => mo.IsHidden == query.IsHidden.Value);
        }

        if (query.Tier.HasValue)
        {
            objectsQuery = objectsQuery.Where(mo => mo.Tier == query.Tier.Value);
        }

        if (query.Version.HasValue)
        {
            objectsQuery = objectsQuery.Where(mo => mo.Version == query.Version.Value);
        }

        // Get total count before pagination
        var totalCount = await objectsQuery.CountAsync(cancellationToken);

        // Apply sorting
        objectsQuery = query.SortBy?.ToLowerInvariant() switch
        {
            "createdat" => query.SortDescending
                ? objectsQuery.OrderByDescending(mo => mo.CreatedAt)
                : objectsQuery.OrderBy(mo => mo.CreatedAt),
            "tier" => query.SortDescending
                ? objectsQuery.OrderByDescending(mo => mo.Tier ?? int.MaxValue)
                : objectsQuery.OrderBy(mo => mo.Tier ?? int.MaxValue),
            _ => query.SortDescending
                ? objectsQuery.OrderByDescending(mo => mo.Name)
                : objectsQuery.OrderBy(mo => mo.Name)
        };

        // Apply pagination
        objectsQuery = objectsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        // Project to DTO with category names
        var objects = await objectsQuery
            .GroupJoin(_queryDbContext.MuseumObjectCategories,
                mo => mo.Id,
                moc => moc.MuseumObjectId,
                (mo, mocGroup) => new { MuseumObject = mo, MuseumObjectCategories = mocGroup })
            .SelectMany(
                x => x.MuseumObjectCategories.DefaultIfEmpty(),
                (x, moc) => new { x.MuseumObject, MuseumObjectCategory = moc })
            .GroupJoin(_queryDbContext.Categories.Where(c => !c.IsDeleted),
                x => x.MuseumObjectCategory != null ? x.MuseumObjectCategory.CategoryId : 0,
                c => c.Id,
                (x, cGroup) => new
                {
                    x.MuseumObject,
                    CategoryNames = cGroup.Select(c => c.Name).ToList()
                })
            .GroupBy(x => x.MuseumObject)
            .Select(g => new AllObjectDto
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                Description = g.Key.Description,
                MuseumId = g.Key.MuseumId,
                QRCode = g.Key.QRCode,
                IsSpecial = g.Key.IsSpecial,
                IsHidden = g.Key.IsHidden,
                Tier = g.Key.Tier,
                Version = g.Key.Version,
                CreatedAt = g.Key.CreatedAt,
                CategoryNames = g.SelectMany(x => x.CategoryNames).Distinct().ToList().AsReadOnly()
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Successfully retrieved {Count} museum objects for query", objects.Count);

        var result = new PagedData<AllObjectDto>(
            items: objects,
            totalCount: totalCount,
            pageSize: query.PageSize,
            currentPage: query.Page
        );
        
        
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while fetching museum objects for query");
        throw;
    }
}
}
