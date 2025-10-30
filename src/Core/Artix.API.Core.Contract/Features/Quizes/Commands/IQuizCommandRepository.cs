namespace Artix.API.Core.Contract.Features.Quizes.Commands;

using Primitives.Repositories;
using Domain.Entities.Quest;
using Domain.Entities.Quiz;

public interface IQuizCommandRepository : ICommandRepository<Quiz>
{
}
