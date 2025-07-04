namespace Artix.API.Core.Contract.Primitives.Handlers;

using MediatR;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, long> 
    where TCommand : ICommand<long> { }

