namespace Artix.API.Infra.Mongo.Repositories.Features.Quiz;

using Core.Contract.Features.Quizes.Commands;
using Core.Domain.Entities.Quest;
using Core.Domain.Entities.Quiz;
using Data.DbContext;
using Primitives;
using Microsoft.Extensions.Logging;

public sealed class QuizCommandRepository : MongoCommandRepository<Quiz>, IQuizCommandRepository
{
    public QuizCommandRepository(MongoCommandContext commandDbContext, ILogger<MongoCommandRepository<Quiz>> logger) :
        base(commandDbContext, logger)
    {
    }
}
