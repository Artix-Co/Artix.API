using FluentValidation;
using Artix.API.Core.Contract.Features.Quizzes.Client.Queries.GetShuffledQuizzes;

namespace Artix.API.Core.ApplicationService.Features.Quizzes.Client.Queries.GetShuffledQuizzes;

public sealed class GetClientShuffledQuizzesQueryValidator : AbstractValidator<GetClientShuffledQuizzesQuery>
{
    public GetClientShuffledQuizzesQueryValidator() => RuleFor(x => x.Count).GreaterThan(0);
}
