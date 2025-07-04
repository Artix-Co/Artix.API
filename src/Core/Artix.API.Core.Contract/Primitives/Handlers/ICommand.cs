namespace Artix.API.Core.Contract.Primitives.Handlers;

using MediatR;

public interface ICommand<out TResponse> : IRequest<TResponse> { }
