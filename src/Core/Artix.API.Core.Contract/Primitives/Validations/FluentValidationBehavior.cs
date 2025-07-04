namespace Artix.API.Core.Contract.Primitives.Validations;

using FluentValidation;
using MediatR;

public sealed class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public FluentValidationBehavior(IValidator<TRequest>? validator = null)
    {
        this._validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only run validation if a validator is available for the request
        if (this._validator != null)
        {
            var validationResult = await this._validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        return await next();
    }
}
