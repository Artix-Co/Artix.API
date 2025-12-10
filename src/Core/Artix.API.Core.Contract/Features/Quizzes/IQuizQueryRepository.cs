namespace Artix.API.Core.Contract.Features.Quizzes;

using Client.Queries.GetShuffledQuizzes;
using Domain.Entities.Quiz;
using Primitives.Repositories;

public interface IQuizQueryRepository : IQueryRepository<Quiz>
{
    ValueTask<IEnumerable<ShuffledQuizzesDto>> GetShuffledAsync(GetShuffledQuizzesQuery dto,
        CancellationToken cancellationToken = default);
}
