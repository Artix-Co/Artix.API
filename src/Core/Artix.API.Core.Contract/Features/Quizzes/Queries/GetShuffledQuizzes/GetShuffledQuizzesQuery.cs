namespace Artix.API.Core.Contract.Features.Quizzes.Queries.GetShuffledQuizzes;

using Primitives.Handlers;

public record GetShuffledQuizzesQuery(int Count = 10) : IQuery<IEnumerable<ShuffledQuizzesDto>>;
