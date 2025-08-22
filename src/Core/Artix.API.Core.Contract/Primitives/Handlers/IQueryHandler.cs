namespace Artix.API.Core.Contract.Primitives.Handlers;

using MediatR;
using Models;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IResponse<Result<TResponse>> { }
