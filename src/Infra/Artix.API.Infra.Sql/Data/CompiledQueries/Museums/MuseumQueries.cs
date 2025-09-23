namespace Artix.API.Infra.Sql.Data.CompiledQueries.Museums;

using Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using DbContexts;
using Microsoft.EntityFrameworkCore;
using File = System.IO.File;

internal static class MuseumQueries
{
    internal static readonly Func<ArtixQueryDbContext, GetAllMuseumsClientQuery, IEnumerable<AllMuseumsClientDto>> GetAllMuseumsClientQuery =
        EF.CompileQuery((ArtixQueryDbContext context, GetAllMuseumsClientQuery dto) =>
            context.Museums
                .Include(o => o.MuseumImages)
                .ThenInclude(of => of.FileEntity)
                .AsSplitQuery()
                .Where(m => string.IsNullOrWhiteSpace(dto.Name) || m.Name.Contains(dto.Name))
                .Select(m => new
                {
                    Museum = m,
                    ImageBase64 = m.MuseumImages
                        .Where(of => of.FileEntity.MimeType == "jpg" ||
                                     of.FileEntity.MimeType == "png" ||
                                     of.FileEntity.MimeType == "jpeg" ||
                                     of.FileEntity.MimeType == "webp")
                        .Select(of => Convert.ToBase64String(File.ReadAllBytes(of.FileEntity.FilePath)))
                        .FirstOrDefault()
                })
                .OrderBy(x => x.Museum.Name)
                .Select(x => new AllMuseumsClientDto(
                    x.Museum.BusinessId,
                    x.Museum.Name,
                    x.ImageBase64,
                    x.Museum.Description,
                    x.Museum.CreatedAt,
                    x.Museum.IsActive
                )));
    
    
    
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
    
    

    
}
