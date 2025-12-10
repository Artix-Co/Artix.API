namespace Artix.API.Core.Contract.Features.Quizzes;

using Primitives.Repositories;
using Domain.Entities.Quiz;

public interface IQuizCommandRepository : ICommandRepository<Quiz>
{
}
