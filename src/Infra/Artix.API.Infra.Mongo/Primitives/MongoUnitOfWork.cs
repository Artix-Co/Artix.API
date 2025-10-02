namespace Artix.API.Infra.Mongo.Primitives;

using System.Threading;
using System.Threading.Tasks;
using Artix.API.Core.Contract.Primitives.Repositories;
using MongoDB.Driver;

public class MongoUnitOfWork : IUnitOfWork
{
    private readonly IMongoClient _client;
    private IClientSessionHandle? _session;

    public MongoUnitOfWork(IMongoClient client)
    {
        this._client = client;
    }

    public void BeginTransaction()
    {
        this._session = this._client.StartSession();
        this._session.StartTransaction();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        this._session = await this._client.StartSessionAsync(cancellationToken: cancellationToken);
        this._session.StartTransaction();
    }

    public void Commit()
    {
        this._session?.CommitTransaction();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await this._session?.CommitTransactionAsync(cancellationToken)!;
    }

    public void Rollback()
    {
        this._session?.AbortTransaction();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await this._session?.AbortTransactionAsync(cancellationToken)!;
    }

    public void Dispose()
    {
        this._session?.Dispose();
    }
}
