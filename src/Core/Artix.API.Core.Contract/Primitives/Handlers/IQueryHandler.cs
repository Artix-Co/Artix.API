namespace Artix.API.Core.Contract.Primitives.Handlers;

using MediatR;

public interface IQueryHandler<in TQuery, TResponse> 
    : IRequestHandler<TQuery, TResponse> 
    where TQuery : IQuery<TResponse> { }
