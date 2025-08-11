namespace Artix.API.Core.Contract.Primitives.Handlers;

using MediatR;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Guid> 
    where TCommand : ICommand { }

