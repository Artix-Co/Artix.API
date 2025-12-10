namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Commands.AddToUserCollection;

using Contract.Features.Collections;
using Contract.Features.Objects;
using Contract.Features.Objects.Client.Commands.AddToUserCollection;
using Contract.Primitives.Repositories;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validation for this handler
internal sealed class AddObjectToUserCollectionCommandHandler : CommandHandlerBase<AddObjectToUserCollectionCommand>
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly ICollectionCommandRepository _collectionCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddObjectToUserCollectionCommandHandler(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IObjectCommandRepository objectCommandRepository,
        ICollectionCommandRepository collectionCommandRepository, IUnitOfWork unitOfWork) : base(httpContextAccessor,
        userManager)
    {
        this._objectCommandRepository = objectCommandRepository;
        this._collectionCommandRepository = collectionCommandRepository;
        this._unitOfWork = unitOfWork;
    }
    public override async Task<Guid> Handle(AddObjectToUserCollectionCommand command, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);

        var collection = await this._collectionCommandRepository.GetByIdAsync(command.CollectionId, cancellationToken);
        if (collection == null || collection.UserId != user.Id)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(collection), command.CollectionId);

        var @object = await this._objectCommandRepository.GetByIdAsync(command.ObjectId, cancellationToken);
        if (@object == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(@object), command.ObjectId);

        @object.AddToCollection(collection);

        await this._unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await this._objectCommandRepository.UpdateAsync(@object, cancellationToken);

            await this._unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await this._unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        return @object.BusinessId;
    }

}
