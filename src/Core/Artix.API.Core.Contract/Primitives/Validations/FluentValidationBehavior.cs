namespace Artix.API.Core.Contract.Primitives.Validations;

using FluentValidation;
using MediatR;

public sealed class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public FluentValidationBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is not null)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var failures = validationResult.Errors
                    .Select(failure => new FluentValidation.Results.ValidationFailure(
                        failure.PropertyName,
                        BilingualValidationMessage.Format(failure)))
                    .ToList();
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
