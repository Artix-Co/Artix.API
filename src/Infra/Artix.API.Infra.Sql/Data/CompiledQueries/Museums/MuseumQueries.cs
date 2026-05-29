namespace Artix.API.Infra.Sql.Data.CompiledQueries.Museums;

using Core.Contract.Features.Museums.Client.Queries.GetAll;
using Core.Contract.Features.Museums.Client.Queries.GetDetailByIds;
using Core.Contract.Features.Museums.Client.Queries.GetObjects;
using Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;
using Core.Contract.Primitives.Models;
using DbContexts;
using Microsoft.EntityFrameworkCore;
using File = System.IO.File;
using Object = Artix.API.Core.Domain.Entities.Object.Object;

internal static class MuseumQueries
{
    internal static readonly Func<ArtixQueryDbContext, Guid, IEnumerable<string>, string, ClientMuseumDetailsByIdDto?>
        GetDetailsByIdQuery =
            EF.CompileQuery((ArtixQueryDbContext context, Guid businessId, IEnumerable<string> allowedImagesTypes,
                    string fileServerBaseUrl) =>
                context.Museums
                    .Where(m => m.IsDeleted == false && m.BusinessId == businessId)
                    .GroupJoin(
                        context.MuseumObjects.Where(mo => mo.Object.IsDeleted == false && mo.Museum.IsDeleted == false),
                        m => m.Id,
                        mo => mo.MuseumId,
                        (m, moGroup) => new
                        {
                            Museum = m,
                            MuseumObjects = moGroup,
                            JournalEntryCount = context.JournalEntries
                                .Count(je => moGroup.Any(mo => mo.ObjectId == je.ObjectId))
                        })
                    .Select(x => new
                    {
                        x.Museum.BusinessId,
                        x.Museum.Name,
                        ImageFilePath = x.Museum.MuseumImages
                            .Where(mi => mi.FileEntity != null &&
                                         !mi.FileEntity.IsDeleted &&
                                         allowedImagesTypes.Contains(mi.FileEntity.MimeType))
                            .Select(mi => mi.FileEntity.FilePath)
                            .FirstOrDefault(),
                        x.Museum.Description,
                        x.Museum.CreatedAt,
                        x.Museum.IsActive,
                        ObjectCount = x.MuseumObjects.Count(),
                        x.JournalEntryCount,
                        x.Museum.Slug
                    })
                    .Select(x => new ClientMuseumDetailsByIdDto(
                        x.BusinessId,
                        x.Name,
                        !string.IsNullOrEmpty(x.ImageFilePath)
                            ? $"{fileServerBaseUrl}/{Path.GetFileName(x.ImageFilePath)}"
                            : null,
                        x.Description,
                        x.CreatedAt,
                        x.IsActive,
                        x.ObjectCount,
                        x.JournalEntryCount,
                        x.Slug
                    ))
                    .FirstOrDefault()
            );

    internal static readonly Func<
        ArtixQueryDbContext,
        string?,
        IEnumerable<string>,
        string,
        IEnumerable<ClientAllMuseumsDto>
    > GetAllMuseumsClientQuery =
        EF.CompileQuery((ArtixQueryDbContext context,
                string? name,
                IEnumerable<string> allowedImagesTypes,
                string fileServerBaseUrl) =>
            context.Museums
                .Where(m => m.IsDeleted == false)
                .Where(m => string.IsNullOrEmpty(name) || m.Name.Contains(name))
                .OrderBy(m => m.Name)
                .Select(m => new
                {
                    m.BusinessId,
                    m.Name,
                    ObjectCount =
                        context.MuseumObjects.Count(mo =>
                            mo.Museum.IsDeleted == false && mo.Object.IsDeleted == false && mo.MuseumId == m.Id),
                    ImageFilePath = context.MuseumImages
                        .Where(mi =>
                            mi.MuseumId == m.Id &&
                            !mi.FileEntity.IsDeleted &&
                            allowedImagesTypes.Contains(mi.FileEntity.MimeType))
                        .Select(mi => mi.FileEntity.FilePath)
                        .FirstOrDefault(),
                    m.Description,
                    m.CreatedAt,
                    m.IsActive,
                    m.Slug
                })
                .Select(x => new ClientAllMuseumsDto(
                    x.BusinessId,
                    x.Name,
                    x.ObjectCount,
                    !string.IsNullOrEmpty(x.ImageFilePath)
                        ? $"{fileServerBaseUrl}/{Path.GetFileName(x.ImageFilePath)}"
                        : null,
                    x.Description,
                    x.CreatedAt,
                    x.IsActive,
                    x.Slug
                ))
        );


    internal static readonly Func<
        ArtixQueryDbContext,
        Guid,
        IEnumerable<string>,
        string,
        IEnumerable<ClientMuseumObjectDto>
    > GetMuseumObjectsQuery =
        EF.CompileQuery(
            (ArtixQueryDbContext context,
                    Guid museumId,
                    IEnumerable<string> allowedImagesTypes,
                    string fileServerBaseUrl) =>
                context.MuseumObjects.Where(mo => mo.Museum.IsDeleted == false && mo.Museum.BusinessId == museumId)
                    .Join(
                        context.Objects,
                        mo => mo.ObjectId,
                        o => o.Id,
                        (mo, o) => new { MuseumObject = mo, Object = o })
                    .Join(
                        context.Museums,
                        x => x.MuseumObject.MuseumId,
                        m => m.Id,
                        (x, m) => new { x.Object, x.MuseumObject, Museum = m })
                    .Select(x => new
                    {
                        Id = x.Object.BusinessId,
                        MuseumId = x.Museum.BusinessId,
                        ImageFilePath = x.Object.ObjectImages
                            .Where(mi => !mi.FileEntity.IsDeleted &&
                                         allowedImagesTypes.Contains(mi.FileEntity.MimeType))
                            .Select(mi => mi.FileEntity.FilePath)
                            .FirstOrDefault(),
                        x.Object.Name,
                        x.Object.Description,
                        x.Object.CreatedAt,
                        x.Object.Slug
                    })
                    .Select(x => new ClientMuseumObjectDto(
                        x.Id,
                        x.MuseumId,
                        !string.IsNullOrEmpty(x.ImageFilePath)
                            ? $"{fileServerBaseUrl}/{Path.GetFileName(x.ImageFilePath)}"
                            : null,
                        x.Name,
                        x.Description,
                        x.CreatedAt,
                        x.Slug
                    ))
        );


    internal static readonly Func<ArtixQueryDbContext, string?, int, int, IEnumerable<ClientPaginateObjectsDto>>
        GetAllObjectsQuery =
            EF.CompileQuery((ArtixQueryDbContext context, string? nameFilter, int pageNumber, int pageSize) =>
                context.Objects
                    .Where(o => string.IsNullOrWhiteSpace(nameFilter) || o.Name.Contains(nameFilter))
                    .OrderBy(o => o.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new ClientPaginateObjectsDto(
                        o.BusinessId,
                        o.Name,
                        o.Description,
                        (from mo in context.MuseumObjects
                            join m in context.Museums on mo.MuseumId equals m.Id
                            where mo.ObjectId == o.Id
                            select m.BusinessId).FirstOrDefault(),
                        o.QrCode,
                        o.IsSpecial,
                        o.IsHidden,
                        o.Tier,
                        o.Version,
                        o.CreatedAt,
                        o.Slug,
                        context.ObjectTypes
                            .Where(ot => ot.ObjectId == o.Id)
                            .Join(context.Types,
                                ot => ot.TypeId,
                                t => t.Id,
                                (ot, t) => new TypeDto(t.BusinessId, t.Name, t.Description))
                            .ToList(),
                        context.HistoricalPeriods
                            .Where(hp => context.ObjectHistoricalPeriods
                                .Any(ohp => ohp.ObjectId == o.Id && ohp.HistoricalPeriodId == hp.Id))
                            .Select(hp => new HistoricalPeriodDto(
                                hp.BusinessId,
                                hp.Name,
                                hp.Description,
                                hp.StartDate,
                                hp.EndDate))
                            .ToList()
                    ))
            );
}
