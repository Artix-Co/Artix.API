namespace Artix.API.Infra.Sql.Repositories.Features.Objects;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Features.Objects;
using Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById;
using Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;
using Core.Domain.Entities.Object;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

public sealed class ObjectQueryRepository : QueryRepository<Object>, IObjectQueryRepository
{
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;
    private readonly string _fileServerBaseUrl;
    private readonly string[] _allowedReadmeMimeTypes;


    public ObjectQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<QueryRepository<Object>> logger,
        IOptions<FileSettings> fileSettingOptions) : base(queryDbContext, logger)
    {
        this._allowed3DMimeTypes = fileSettingOptions.Value.Allowed3DMimeTypes;
        this._allowedImageMimeTypes = fileSettingOptions.Value.AllowedImageMimeTypes;
        this._fileServerBaseUrl = fileSettingOptions.Value.BaseUrl;
        this._allowedReadmeMimeTypes = fileSettingOptions.Value.AllowedReadmeMimeTypes;
    }

    public async Task<ClientObjectDetailsByIdDto> GetDetailsByIdAsync(
        GetClientObjectDetailsByIdQuery dto,
        CancellationToken cancellationToken = default)
    {
        var query = await _queryDbContext.Objects
            .Where(o => o.BusinessId == dto.Id)
            .Select(o => new
            {
                o.BusinessId,
                o.Name,
                o.GeneralInformation,
                o.SpecialInformation,
                Model3DFilePath = o.ObjectModels
                    .Where(of => !of.FileEntity.IsDeleted &&
                                 this._allowed3DMimeTypes.Contains(of.FileEntity.MimeType))
                    .Select(of => of.FileEntity.FilePath)
                    .FirstOrDefault(),
                ImageFilePath = o.ObjectImages
                    .Where(of => !of.FileEntity.IsDeleted &&
                                 this._allowedImageMimeTypes.Contains(of.FileEntity.MimeType))
                    .Select(of => of.FileEntity.FilePath)
                    .FirstOrDefault(),
                GeneralInformationFilePath = o.ObjectGeneralInformation
                    .Where(of => !of.FileEntity.IsDeleted &&
                                 this._allowedReadmeMimeTypes.Contains(of.FileEntity.MimeType))
                    .Select(of => of.FileEntity.FilePath)
                    .FirstOrDefault(),
                
                SpecialInformationFilePath = o.ObjectGeneralInformation
                    .Where(of => !of.FileEntity.IsDeleted &&
                                 this._allowedReadmeMimeTypes.Contains(of.FileEntity.MimeType))
                    .Select(of => of.FileEntity.FilePath)
                    .FirstOrDefault(),
                
                HistoricalPeriods = o.ObjectHistoricalPeriods
                    .Select(ohp => new HistoricalPeriodDto(
                        ohp.HistoricalPeriod.BusinessId,
                        ohp.HistoricalPeriod.Name,
                        ohp.HistoricalPeriod.Description,
                        ohp.HistoricalPeriod.StartDate,
                        ohp.HistoricalPeriod.EndDate
                    ))
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        if (query is null)
            throw InfrastructureNotFoundException.ForEntity(nameof(Object), dto.Id);

        var model3DUrl = !string.IsNullOrEmpty(query.Model3DFilePath)
            ? $"{_fileServerBaseUrl}/{Path.GetFileName(query.Model3DFilePath)}"
            : null;

        var imageUrl = !string.IsNullOrEmpty(query.ImageFilePath)
            ? $"{_fileServerBaseUrl}/{Path.GetFileName(query.ImageFilePath)}"
            : null;
        
        var generalInformationUrl = !string.IsNullOrEmpty(query.GeneralInformationFilePath)
            ? $"{_fileServerBaseUrl}/{Path.GetFileName(query.GeneralInformationFilePath)}"
            : null;
        
        var specialInformationUrl = !string.IsNullOrEmpty(query.SpecialInformationFilePath)
            ? $"{_fileServerBaseUrl}/{Path.GetFileName(query.SpecialInformationFilePath)}"
            : null;

        return new ClientObjectDetailsByIdDto(
            Id: query.BusinessId,
            Name: query.Name,
            GeneralInformation: query.GeneralInformation,
            SpecialInformation: query.SpecialInformation,
            Model3DUrl: model3DUrl,
            ImageUrl: imageUrl,
            GeneralInformationUrl: generalInformationUrl,
            SpecialInformationUrl:specialInformationUrl,
            HistoricalPeriods: query.HistoricalPeriods.ToList()
        );
    }


 
    public async Task<Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById.ObjectDetailsByIdDto> GetObjectDetailsByIdAdminAsync(
        Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById.GetObjectDetailsByIdQuery dto, CancellationToken cancellationToken = default)
    {
        // Validate input
        if (dto?.Id == null)
        {
            throw new ArgumentNullException(nameof(dto.Id), "Object ID cannot be null.");
        }

        // Log the query input
        _logger.LogInformation("Querying object with BusinessId: {BusinessId}", dto.Id);

        // Fetch data from database
        var query = await _queryDbContext.Objects
            .Include(o => o.ObjectModels)
            .ThenInclude(of => of.FileEntity)
            .Include(o => o.ObjectImages)
            .ThenInclude(of => of.FileEntity)
            .Include(o => o.ObjectHistoricalPeriods)
            .ThenInclude(ohp => ohp.HistoricalPeriod)
            .Include(o => o.ObjectTypes)
            .ThenInclude(ot => ot.Category)
            .FirstOrDefaultAsync(o => o.BusinessId == dto.Id, cancellationToken);

        if (query == null)
        {
            _logger.LogWarning("No object found for BusinessId: {BusinessId}", dto.Id);
            throw InfrastructureNotFoundException.ForEntity(nameof(Object), dto.Id);
        }

        // Log Object3DModels details
        _logger.LogInformation("Object3DModels count: {Count}", query.ObjectModels?.Count() ?? 0);
        if (query.ObjectModels != null && query.ObjectModels.Any())
        {
            foreach (var object3DModel in query.ObjectModels)
            {
                _logger.LogInformation(
                    "Model: ObjectId={ObjectId}, FileId={FileId}, MimeType={MimeType}, FilePath={FilePath}",
                    object3DModel.ObjectId, object3DModel.FileId, object3DModel.FileEntity?.MimeType,
                    object3DModel.FileEntity?.FilePath);
            }
        }
        else
        {
            _logger.LogWarning("No Object3DModels found for BusinessId: {BusinessId}", dto.Id);
        }


        // Process 3D model file
        string model3DBase64 = "";


        // Process image file
        string imageBase64 = "";


        // Map related entities to DTOs
        var objectTypes = query.ObjectTypes
            .Select(ot => new TypeDto(
                Id: ot.Category.BusinessId,
                Name: ot.Category.Name,
                Description: ot.Category.Description))
            .ToList();

        var historicalPeriods = query.ObjectHistoricalPeriods
            .Select(ohp => new HistoricalPeriodDto(
                Id: ohp.HistoricalPeriod.BusinessId,
                Name: ohp.HistoricalPeriod.Name,
                Description: ohp.HistoricalPeriod.Description,
                StartDate: ohp.HistoricalPeriod.StartDate,
                EndDate: ohp.HistoricalPeriod.EndDate))
            .ToList();

        // Return DTO
        return new Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById.ObjectDetailsByIdDto(
            Id: query.BusinessId,
            Name: query.Name,
            GeneralInformation: query.GeneralInformation,
            SpecialInformation: query.SpecialInformation,
            Version: query.Version,
            Tier: query.Tier,
            IsSpecial: query.IsSpecial,
            IsHidden: query.IsHidden,
            ObjectSaleType: query.ObjectSaleType,
            CreatedAt: query.CreatedAt,
            ImageBase64: imageBase64,
            Model3DBase64: model3DBase64,
            ObjectTypes: objectTypes,
            HistoricalPeriods: historicalPeriods);
    }
}
