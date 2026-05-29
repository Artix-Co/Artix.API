namespace Artix.API.Infra.Sql.Repositories.Features.Museums;

using Artix.API.Core.Contract.Primitives.Models;
using Core.Domain.Entities.Museum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract.Configs.FileSettings;
using Core.Contract.Features.Museums;
using Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;
using Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects;
using Core.Contract.Features.Museums.Client.Queries.GetAll;
using Core.Contract.Features.Museums.Client.Queries.GetDetailByIds;
using Core.Contract.Features.Museums.Client.Queries.GetJournalEntries;
using Core.Contract.Features.Museums.Client.Queries.GetKeyStatus;
using Core.Contract.Features.Museums.Client.Queries.GetObjects;
using Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;
using Data.CompiledQueries.Museums;
using Data.DbContexts;
using DPG.Core.Contract.Primitives.Models;
using Exceptions;
using Microsoft.Extensions.Options;
using Primitives;

public sealed class MuseumQueryRepository : QueryRepository<Museum>, IMuseumQueryRepository
{
    private readonly string _fileServerBaseUrl;
    private readonly string[] _allowedImageMimeTypes;


    public MuseumQueryRepository(ArtixQueryDbContext queryDbContext, IOptions<FileSettings> fileSettingOptions,
        ILogger<QueryRepository<Museum>> logger) : base(queryDbContext, logger)
    {
        this._allowedImageMimeTypes = fileSettingOptions.Value.AllowedImageMimeTypes;
        this._fileServerBaseUrl = fileSettingOptions.Value.BaseUrl;
    }

    public IEnumerable<ClientAllMuseumsDto> GetAllMuseumsClient(GetClientAllMuseumsQuery dto)
    {
        _logger.LogInformation("Fetching all museums with query: {@Query}", dto);

        var museums = MuseumQueries.GetAllMuseumsClientQuery(
            _queryDbContext,
            dto.Name,
            _allowedImageMimeTypes,
            _fileServerBaseUrl
        );

        var museumsList = museums.ToArray();

        if (museumsList.Length == 0)
        {
            throw InfrastructureNotFoundException.WithMessage("No museums found!");
        }

        return museumsList;
    }


    public IEnumerable<ClientMuseumObjectDto> GetObjects(GetClientMuseumObjectsQuery dto)
    {
        _logger.LogInformation("Fetching objects for museum ID: {MuseumId}", dto.MuseumId);

        var objects = MuseumQueries.GetMuseumObjectsQuery(_queryDbContext, dto.MuseumId, this._allowedImageMimeTypes,
            this._fileServerBaseUrl);

        return objects;
    }

    public async Task<PaginatedResult<AdminMuseumObjectDto>> GetAdminObjectsAsync(
        GetAdminMuseumObjectsQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching admin objects for MuseumId: {MuseumId}", dto.MuseumId);

        var pageNumber = Math.Max(dto.PageNumber, 1);
        var pageSize = Math.Max(dto.PageSize, 1);


        var query =
            from m in _queryDbContext.Museums
            where m.IsDeleted == false && m.BusinessId == dto.MuseumId
            join mo in _queryDbContext.MuseumObjects on m.Id equals mo.MuseumId
            join obj in _queryDbContext.Objects on mo.ObjectId equals obj.Id
            join objImg in _queryDbContext.ObjectImages on obj.Id equals objImg.ObjectId
            where objImg.FileEntity != null
                  && !objImg.FileEntity.IsDeleted
                  && _allowedImageMimeTypes.Contains(objImg.FileEntity.MimeType)
            select new
            {
                m,
                mo,
                obj,
                ObjectImagePath = obj
                    .ObjectImages
                    .Where(oi =>
                        oi.FileEntity != null && !oi.FileEntity.IsDeleted &&
                        _allowedImageMimeTypes.Contains(oi.FileEntity.MimeType))
                    .Select(oi => oi.FileEntity.FilePath)
                    .FirstOrDefault()
            };


        if (!string.IsNullOrWhiteSpace(dto.NameFilter))
        {
            var lowerName = dto.NameFilter.ToLower();
            query = query.Where(x =>
                x.obj.Name.ToLower().Contains(lowerName) || x.obj.Name.ToLower().Contains(lowerName));
        }

        if (dto.IsSpecial.HasValue)
        {
            query = query.Where(x => x.obj.IsSpecial == dto.IsSpecial.Value);
        }

        if (dto.IsHidden.HasValue)
        {
            query = query.Where(x => x.obj.IsHidden == dto.IsHidden.Value);
        }

        if (dto.Tier.HasValue)
        {
            query = query.Where(x => x.obj.Tier == dto.Tier.Value);
        }

        if (dto.Version.HasValue)
        {
            query = query.Where(x => x.obj.Version == dto.Version.Value);
        }


        // Sorting
        query = (dto.SortBy?.ToLower() ?? "createdat") switch
        {
            "name" => dto.SortDescending
                ? query.OrderByDescending(x => x.obj.Name)
                : query.OrderBy(x => x.obj.Name),

            "tier" => dto.SortDescending
                ? query.OrderByDescending(x => x.obj.Tier)
                : query.OrderBy(x => x.obj.Tier),

            "version" => dto.SortDescending
                ? query.OrderByDescending(x => x.obj.Version)
                : query.OrderBy(x => x.obj.Version),

            "isspecial" => dto.SortDescending
                ? query.OrderByDescending(x => x.obj.IsSpecial)
                : query.OrderBy(x => x.obj.IsSpecial),

            _ => dto.SortDescending
                ? query.OrderByDescending(x => x.obj.CreatedAt)
                : query.OrderBy(x => x.obj.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminMuseumObjectDto(
                x.obj.BusinessId,
                !string.IsNullOrEmpty(x.ObjectImagePath)
                    ? $"{_fileServerBaseUrl}/{Path.GetFileName(x.ObjectImagePath)}"
                    : null,
                x.obj.Name,
                x.obj.Description,
                x.obj.CreatedAt,
                x.obj.Slug
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AdminMuseumObjectDto>(
            Items: items,
            TotalCount: totalCount,
            PageNumber: pageNumber,
            PageSize: pageSize,
            Draw: true
        );
    }

    public ClientMuseumDetailsByIdDto GetDetailsById(GetClientMuseumDetailsByIdQuery dto)
    {
        _logger.LogInformation("Fetching museum with ID: {MuseumId}", dto.Id);

        var museum =
            MuseumQueries.GetDetailsByIdQuery(this._queryDbContext, dto.Id, this._allowedImageMimeTypes,
                _fileServerBaseUrl);

        if (museum == null)
        {
            throw InfrastructureNotFoundException.ForEntity(nameof(Museum), dto.Id);
        }

        return museum;
    }

    public async Task<PaginatedResult<ClientPaginateObjectsDto>> GetAllObjectsAsync(GetClientPaginateObjectsQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching objects with query: {@Query}", dto);

        var objects = MuseumQueries.GetAllObjectsQuery(
            _queryDbContext,
            dto.NameFilter,
            dto.PageNumber,
            dto.PageSize
        ).ToArray().AsReadOnly();

        var totalCount = await _queryDbContext.Objects
            .Where(o => string.IsNullOrWhiteSpace(dto.NameFilter) || o.Name.Contains(dto.NameFilter))
            .CountAsync(cancellationToken);

        return new PaginatedResult<ClientPaginateObjectsDto>(
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


    public async Task<PaginatedResult<AdminPaginatedMuseumsDto>> GetAllMuseumsAdminAsync(
        GetAdminPaginateMuseumsQuery dto,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(dto.PageNumber, 1);
        var pageSize = Math.Max(dto.PageSize, 1);

        var query = _queryDbContext.Museums.Where(m => m.IsDeleted == false)
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
            .Select(m => new AdminPaginatedMuseumsDto(
                m.BusinessId,
                m.Name,
                m.Description,
                m.CreatedAt,
                m.IsActive,
                m.Slug))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AdminPaginatedMuseumsDto>(
            Items: pagedItems,
            TotalCount: totalCount,
            PageNumber: pageNumber,
            Draw: true,
            PageSize: pageSize
        );
    }
}
