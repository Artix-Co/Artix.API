namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetDetailByIds;

using Contract.Features.Objects.Queries;
using Contract.Features.Objects.Queries.GetDetailByIds;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetObjectDetailByIdQueryHandler : QueryHandlerBase<GetObjectDetailByIdQuery, ObjectDetailByIdDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;

    public GetObjectDetailByIdQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        IObjectQueryRepository objectQueryRepository) : base(cache, httpContextAccessor)
    {
        this._objectQueryRepository = objectQueryRepository;
    }

    public override async Task<ObjectDetailByIdDto> Handle(GetObjectDetailByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = new ObjectDetailByIdDto();

        var museumObject = await this._objectQueryRepository.GetByIdAsync(query.Id, cancellationToken);

        if (museumObject == null)
        {
            // TODO: convert it to ApplicationServiceNotFoundException.ForEntity
            throw new KeyNotFoundException("The given object could not be found.");
        }

        result.Name = museumObject.Name;
        result.GeneralInformation = museumObject.GeneralInformation;
        result.SpecializedInformation = museumObject.SpecialInformation;
        result.HistoricalPeriod = "دوره آرتیکسیان";
        result.Model3DBase64 = "base64-string-model";

        return result;
    }
}
