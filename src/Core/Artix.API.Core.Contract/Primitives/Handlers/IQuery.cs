namespace Artix.API.Core.Contract.Primitives.Handlers;

using MediatR;
using Models;

public interface IResponse<out TResponse> : IRequest<TResponse>
{
}

public interface IQuery<TResponse> : IResponse<Result<TResponse>>
{
}
