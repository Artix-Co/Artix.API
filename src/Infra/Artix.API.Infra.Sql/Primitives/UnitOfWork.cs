namespace Artix.API.Infra.Sql.Primitives;

using Core.Contract.Primitives.Repositories;
using Data;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly ArtixCommandDbContext _commandDbContext;

    private IDbContextTransaction _transaction;
    private bool _disposed = false;

    public UnitOfWork(ArtixCommandDbContext commandDbContext)
    {
        this._commandDbContext = commandDbContext;
    }

    #region Sync Methods

    public void BeginTransaction()
    {
        this._transaction = this._commandDbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        try
        {
            this._commandDbContext.SaveChanges();
            this._transaction.Commit();
        }
        catch
        {
            this._transaction.Rollback();
            throw;
        }
    }

    public void Rollback()
    {
        this._transaction?.Rollback();
    }

    #endregion

    #region Async Methods

    public async Task BeginTransactionAsync()
    {
        this._transaction = await this._commandDbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await this._commandDbContext.SaveChangesAsync();
            await this._transaction.CommitAsync();
        }
        catch
        {
            await this._transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        if (this._transaction != null)
        {
            await this._transaction.RollbackAsync();
        }
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._transaction?.Dispose();
                this._commandDbContext?.Dispose();
            }
            this._disposed = true;
        }
    }

    #endregion
}
