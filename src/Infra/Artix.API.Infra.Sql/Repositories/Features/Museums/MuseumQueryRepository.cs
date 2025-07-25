namespace Artix.API.Infra.Sql.Repositories.Features.Museums;

using System.Linq.Expressions;
using Core.Contract.Features.Museums.Queries;
using Core.Contract.Features.Museums.Queries.GetAll;
using Core.Contract.Features.Museums.Queries.GetById;
using Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Features.Museums.Queries.GetObjectScans;
using Core.Contract.Primitives.Models;
using Core.Domain.Entities.Museum;
using Data;
using Data.DbContexts;
using Exceptions;
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

    public async Task<IEnumerable<AllMuseumDto>> GetAllAsync(GetAllMuseumsQuery dto,
        CancellationToken cancellationToken = default)
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


    public async Task<MuseumByIdDto?> GetDetailsByIdAsync(GetMuseumByIdQuery dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching museum with ID: {MuseumId}", dto.Id);

            var museum = await _queryDbContext.Museums
                .Where(m => m.Id == dto.Id && !m.IsDeleted)
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
                .Where(mo => mo.MuseumId == dto.MuseumId && !mo.IsDeleted)
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

            var journalEntries = await _queryDbContext.Museums
                .Where(m => m.Id == dto.MuseumId && !m.IsDeleted)
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
                        UserId =
                            je.UserJournalEntries.Any()
                                ? je.UserJournalEntries.First().UserId
                                : 0, // Adjust based on actual relationship
                        Content = je.Notes, // Assuming Notes is the content field
                        CreatedAt = je.CreatedAt,
                        Title = je.Title,
                        SketchUrl = je.SketchUrl
                    })
                .OrderByDescending(dto => dto.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} journal entries for museum ID {MuseumId}",
                journalEntries.Count, dto.MuseumId);
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
                _logger.LogInformation("No key found for museum ID {MuseumId} and user ID {UserId}", dto.MuseumId,
                    dto.UserId);
                return new MuseumKeyStatusDto
                {
                    MuseumId = dto.MuseumId,
                    UserId = dto.UserId,
                    HasKey = false,
                    GrantedAt = null,
                    ExpiresAt = null
                };
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
            _logger.LogInformation("Fetching museum objects with query: {@Query}", dto);

            var objectsQuery = _queryDbContext.MuseumObjects
                .Where(mo => !mo.IsDeleted);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(dto.NameFilter))
            {
                objectsQuery = objectsQuery.Where(mo => mo.Name.Contains(dto.NameFilter));
            }

            if (dto.MuseumId.HasValue)
            {
                objectsQuery = objectsQuery.Where(mo => mo.MuseumId == dto.MuseumId.Value);
            }

            if (dto.CategoryIds.Any())
            {
                objectsQuery = objectsQuery.Where(mo => mo.MuseumObjectCategories
                    .Any(moc => dto.CategoryIds.Contains(moc.CategoryId)));
            }

            if (dto.IsSpecial.HasValue)
            {
                objectsQuery = objectsQuery.Where(mo => mo.IsSpecial == dto.IsSpecial.Value);
            }

            if (dto.IsHidden.HasValue)
            {
                objectsQuery = objectsQuery.Where(mo => mo.IsHidden == dto.IsHidden.Value);
            }

            if (dto.Tier.HasValue)
            {
                objectsQuery = objectsQuery.Where(mo => mo.Tier == dto.Tier.Value);
            }

            if (dto.Version.HasValue)
            {
                objectsQuery = objectsQuery.Where(mo => mo.Version == dto.Version.Value);
            }

            // Get total count before pagination
            var totalCount = await objectsQuery.CountAsync(cancellationToken);

            // Apply sorting
            objectsQuery = dto.SortBy?.ToLowerInvariant() switch
            {
                "createdat" => dto.SortDescending
                    ? objectsQuery.OrderByDescending(mo => mo.CreatedAt)
                    : objectsQuery.OrderBy(mo => mo.CreatedAt),
                "tier" => dto.SortDescending
                    ? objectsQuery.OrderByDescending(mo => mo.Tier ?? int.MaxValue)
                    : objectsQuery.OrderBy(mo => mo.Tier ?? int.MaxValue),
                _ => dto.SortDescending
                    ? objectsQuery.OrderByDescending(mo => mo.Name)
                    : objectsQuery.OrderBy(mo => mo.Name)
            };

            // Apply pagination
            objectsQuery = objectsQuery
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize);

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
                    (x, cGroup) => new { x.MuseumObject, CategoryNames = cGroup.Select(c => c.Name).ToList() })
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
                pageSize: dto.PageSize,
                currentPage: dto.Page
            );


            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching museum objects for query");
            throw;
        }
    }

    
    public async Task<ObjectScanDto> GetObjectScanAsync(GetObjectScanQuery dto, CancellationToken cancellationToken)
    {
        var result = await (
            from mo in _queryDbContext.MuseumObjects
            where mo.Id == dto.Id

            join m in _queryDbContext.Museums
                on mo.MuseumId equals m.Id

            join moc in _queryDbContext.MuseumObjectCategories
                on mo.Id equals moc.MuseumObjectId into mocGroup
            from moc in mocGroup.DefaultIfEmpty()

            join c in _queryDbContext.Categories
                on moc.CategoryId equals c.Id into cGroup
            from c in cGroup.DefaultIfEmpty()

            join mt in _queryDbContext.MusicTracks
                on mo.Id equals mt.MuseumObjectId into mtGroup
            from mt in mtGroup.DefaultIfEmpty()

            group new { c, mt } by new
            {
                mo.Id,
                mo.Name,
                mo.QRCode,
                mo.Description,
                mo.Version,
                mo.Tier,
                mo.IsSpecial,
                mo.IsHidden,
                mo.MuseumId,
                MuseumName = m.Name
            } into g

            select new ObjectScanDto
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                QrCode = g.Key.QRCode,
                Description = g.Key.Description,
                Version = g.Key.Version,
                Tier = g.Key.Tier,
                IsSpecial = g.Key.IsSpecial,
                IsHidden = g.Key.IsHidden,
                MuseumId = g.Key.MuseumId,
                MuseumName = g.Key.MuseumName,

                VoiceAssistantAudio = g
                    .Select(x => x.mt?.Url)
                    .FirstOrDefault(url => !string.IsNullOrWhiteSpace(url)),

                VoiceAssistantTitle = g.Select(x => x.mt?.Title).FirstOrDefault(),
                VoiceAssistantIsFree = g.Select(x => x.mt?.IsFree).FirstOrDefault(),
                VoiceAssistantArtist = g.Select(x => x.mt?.Artist).FirstOrDefault(),
                
                Categories = g
                    .Select(x => x.c)
                    .Where(c => c != null)
                    .ToList()
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
            throw InfrastructureNotFoundException.ForEntity("MuseumObject", dto.Id);

        return result;
    }

}
