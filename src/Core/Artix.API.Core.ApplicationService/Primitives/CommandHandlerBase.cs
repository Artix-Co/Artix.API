namespace Artix.API.Core.ApplicationService.Primitives;


using Contract.Primitives.Handlers;
using Microsoft.AspNetCore.Http;

public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand<long>
{
    private readonly IHttpContextAccessor _httpContextAccessor; // Non-static field

    protected CommandHandlerBase(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor; // Assign it here
    }

    public abstract Task<long> Handle(TCommand command, CancellationToken cancellationToken);
}
