namespace Artix.API.Infra.Sql.Data.CompiledQueries.Museums;

using Core.Contract.Features.Museums.Client.Queries.GetAll;
using Core.Contract.Features.Museums.Client.Queries.GetDetailByIds;
using Core.Contract.Features.Museums.Client.Queries.GetObjects;
using Core.Contract.Features.Objects.Client.Queries.GetAll;
using Core.Contract.Primitives.Models;
using DbContexts;
using Microsoft.EntityFrameworkCore;
using File = System.IO.File;
using Object = Artix.API.Core.Domain.Entities.Object.Object;

internal static class MuseumQueries
{
    internal static readonly Func<
        ArtixQueryDbContext,
        string?,
        IEnumerable<string>,
        string,
        IEnumerable<AllMuseumsDto>
    > GetAllMuseumsClientQuery =
        EF.CompileQuery((ArtixQueryDbContext context,
                string? name,
                IEnumerable<string> allowedImagesTypes,
                string fileServerBaseUrl) =>
            context.Museums
                .Where(m => string.IsNullOrEmpty(name) || m.Name.Contains(name))
                .OrderBy(m => m.Name)
                .Select(m => new
                {
                    m.BusinessId,
                    m.Name,
                    ObjectCount = context.MuseumObjects.Count(o => o.MuseumId == m.Id),
                    ImageFilePath = context.MuseumImages
                        .Where(mi =>
                            mi.MuseumId == m.Id &&
                            !mi.FileEntity.IsDeleted &&
                            allowedImagesTypes.Contains(mi.FileEntity.MimeType))
                        .Select(mi => mi.FileEntity.FilePath)
                        .FirstOrDefault(),
                    m.Description,
                    m.CreatedAt,
                    m.IsActive
                })
                .Select(x => new AllMuseumsDto(
                    x.BusinessId,
                    x.Name,
                    x.ObjectCount,
                    !string.IsNullOrEmpty(x.ImageFilePath)
                        ? $"{fileServerBaseUrl}/{Path.GetFileName(x.ImageFilePath)}"
                        : null,
                    x.Description,
                    x.CreatedAt,
                    x.IsActive
                ))
        );


    internal static readonly Func<
        ArtixQueryDbContext,
        Guid,
        IEnumerable<string>,
        string,
        IEnumerable<MuseumObjectDto>
    > GetMuseumObjectsQuery =
        EF.CompileQuery(
            (ArtixQueryDbContext context,
                    Guid museumId,
                    IEnumerable<string> allowedImagesTypes,
                    string fileServerBaseUrl) =>
                context.MuseumObjects
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
                    .Where(x => x.Museum.BusinessId == museumId)
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
                        Description = x.Object.GeneralInformation,
                        x.Object.CreatedAt
                    })
                    .Select(x => new MuseumObjectDto(
                        x.Id,
                        x.MuseumId,
                        !string.IsNullOrEmpty(x.ImageFilePath)
                            ? $"{fileServerBaseUrl}/{Path.GetFileName(x.ImageFilePath)}"
                            : null,
                        x.Name,
                        x.Description,
                        x.CreatedAt
                    ))
        );


    internal static readonly Func<ArtixQueryDbContext, Guid, IEnumerable<string>, string, MuseumDetailsByIdDto?>
        GetDetailsByIdQuery =
            EF.CompileQuery((ArtixQueryDbContext context, Guid businessId, IEnumerable<string> allowedImagesTypes,
                    string fileServerBaseUrl) =>
                context.Museums
                    .Where(m => m.BusinessId == businessId)
                    .GroupJoin(
                        context.MuseumObjects,
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
                        x.JournalEntryCount
                    })
                    .Select(x => new MuseumDetailsByIdDto(
                        x.BusinessId,
                        x.Name,
                        !string.IsNullOrEmpty(x.ImageFilePath)
                            ? $"{fileServerBaseUrl}/{Path.GetFileName(x.ImageFilePath)}"
                            : null,
                        x.Description,
                        x.CreatedAt,
                        x.IsActive,
                        x.ObjectCount,
                        x.JournalEntryCount
                    ))
                    .FirstOrDefault()
            );

    internal static readonly Func<ArtixQueryDbContext, string?, int, int, IEnumerable<AllObjectDto>>
        GetAllObjectsQuery =
            EF.CompileQuery((ArtixQueryDbContext context, string? nameFilter, int pageNumber, int pageSize) =>
                context.Objects
                    .Where(o => string.IsNullOrWhiteSpace(nameFilter) || o.Name.Contains(nameFilter))
                    .OrderBy(o => o.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new AllObjectDto(
                        o.BusinessId,
                        o.Name,
                        o.GeneralInformation,
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
