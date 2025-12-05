namespace Artix.API.Infra.Sql.Data.CompiledQueries.Museums;

using Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;
using Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Primitives.Models;
using DbContexts;
using Microsoft.EntityFrameworkCore;
using File = System.IO.File;
using Object = Artix.API.Core.Domain.Entities.Object.Object;

internal static class MuseumQueries
{
    internal static readonly
        Func<ArtixQueryDbContext, GetAllMuseumsClientQuery, IEnumerable<AllMuseumsClientDto>>
        GetAllMuseumsClientQuery =
            EF.CompileQuery((ArtixQueryDbContext context, GetAllMuseumsClientQuery dto) =>
                context.Museums
                    .Where(m => string.IsNullOrEmpty(dto.Name) || m.Name.Contains(dto.Name))
                    .OrderBy(m => m.Name)
                    .Include(m => m.MuseumImages)
                    .ThenInclude(mi => mi.FileEntity)
                    .AsSplitQuery()
                    .Select(m => new AllMuseumsClientDto(
                        m.BusinessId,
                        m.Name,
                        m.MuseumObjects.Count,
                        m.MuseumImages
                            .Where(mi =>
                                mi.FileEntity.MimeType == "jpg" ||
                                mi.FileEntity.MimeType == "png" ||
                                mi.FileEntity.MimeType == "jpeg" ||
                                mi.FileEntity.MimeType == "webp")
                            .Select(mi => mi.FileEntity.FilePath)
                            .FirstOrDefault(),
                        m.Description,
                        m.CreatedAt,
                        m.IsActive
                    ))
            );


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


    internal static readonly Func<ArtixQueryDbContext, Guid, IEnumerable<MuseumObjectDto>> GetMuseumObjectsQuery =
        EF.CompileQuery((ArtixQueryDbContext context, Guid museumId) =>
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
                .Select(x => new MuseumObjectDto(
                    x.Object.BusinessId,
                    x.Museum.BusinessId,
                    x.Object.Name,
                    x.Object.GeneralInformation,
                    x.Object.CreatedAt
                )));


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
