namespace Artix.API.Infra.Mongo.Repositories.Features.Quests;

using Core.Contract.Features.Quests.Commands;
using Core.Domain.Entities.Quest;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Primitives;

public sealed class QuestCommandRepository:MongoCommandRepository<Quest>, IQuestCommandRepository
{
    public QuestCommandRepository(IMongoDatabase database, ILogger<MongoCommandRepository<Quest>> logger) : base(database, logger)
    {
    }
}
