namespace Artix.API.Core.Contract.Features.Quizzes.Client.Queries.GetShuffledQuizzes;

using Primitives.Handlers;

public sealed record GetClientShuffledQuizzesQuery(int Count = 10) : IQuery<IEnumerable<ClientShuffledQuizzesDto>>;
