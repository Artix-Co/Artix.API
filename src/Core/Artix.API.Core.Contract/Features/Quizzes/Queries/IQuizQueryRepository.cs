namespace Artix.API.Core.Contract.Features.Quizzes.Queries;

using Domain.Entities.Quiz;
using GetShuffledQuizzes;
using Primitives.Repositories;

public interface IQuizQueryRepository : IQueryRepository<Quiz>
{
    ValueTask<IEnumerable<ShuffledQuizzesDto>> GetShuffledAsync(GetShuffledQuizzesQuery dto,
        CancellationToken cancellationToken = default);
}
