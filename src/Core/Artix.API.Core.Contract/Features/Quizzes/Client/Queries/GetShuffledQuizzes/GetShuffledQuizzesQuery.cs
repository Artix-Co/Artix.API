namespace Artix.API.Core.Contract.Features.Quizzes.Client.Queries.GetShuffledQuizzes;

using Primitives.Handlers;

public sealed record GetShuffledQuizzesQuery(int Count = 10) : IQuery<IEnumerable<ShuffledQuizzesDto>>;
