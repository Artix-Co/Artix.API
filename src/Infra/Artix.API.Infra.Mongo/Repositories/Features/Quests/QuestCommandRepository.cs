namespace Artix.API.Infra.Mongo.Repositories.Features.Quests;

using Core.Contract.Features.Quests.Commands;
using Core.Domain.Entities.Quest;
using Data.DbContext;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Primitives;

public sealed class QuestCommandRepository:MongoCommandRepository<Quest>, IQuestCommandRepository
{
    public QuestCommandRepository(MongoCommandContext commandDbContext, ILogger<MongoCommandRepository<Quest>> logger) : base(commandDbContext, logger)
    {
    }
}
