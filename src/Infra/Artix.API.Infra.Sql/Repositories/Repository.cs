namespace Artix.API.Infra.Sql.Repositories;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ArtixCommandDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly ILogger<Repository<T>> _logger;

    public Repository(ArtixCommandDbContext context, ILogger<Repository<T>> logger)
    {
        this._context = context;
        this._dbSet = this._context.Set<T>();
        this._logger = logger;
    }

    #region Async methods

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        this._logger.LogInformation("Getting entity of type {EntityType} by id {Id}", typeof(T).Name, id);

        T entity;

        entity = await this._dbSet.FindAsync(id);

        if (entity == null)
        {
            this._logger.LogWarning("Entity of type {EntityType} with id {Id} not found", typeof(T).Name, id);
        }
        else
        {
            this._logger.LogInformation("Entity of type {EntityType} with id {Id} found in database", typeof(T).Name, id);
        }

        return entity;
    }

    public virtual async Task AddAsync(T entity)
    {
        this._logger.LogInformation("Adding entity of type {EntityType}", typeof(T).Name);
        await this._dbSet.AddAsync(entity);
        this._logger.LogInformation("Entity of type {EntityType} added");

        await this.CommitAsync();
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entity)
    {
        this._logger.LogInformation("Adding range entity of type {EntityType}", typeof(T).Name);
        await this._dbSet.AddRangeAsync(entity);
        this._logger.LogInformation("Entity of type {EntityType} added");

        await this.CommitAsync();
    }
    
    
    public virtual async Task<int> CommitAsync()
    {
        this._logger.LogInformation("Committing transaction asynchronously");
        var result = await this._context.SaveChangesAsync();
        this._logger.LogInformation("Transaction committed asynchronously");
        return result;
    }

    public async Task UpdateAsync(T entity)
    {
        this._logger.LogInformation("Updating entity of type {EntityType}", typeof(T).Name);

        if (entity is BaseEntity baseEntity)
        {
            await this._dbSet
                .Where(e => e.Equals(entity))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => (e as BaseEntity).IsDeleted, true)
                );

            this._logger.LogInformation("Entity of type {EntityType} updated", typeof(T).Name);
            await this.CommitAsync();
        }
        else
        {
            this._logger.LogWarning("Entity of type {EntityType} does not inherit from BaseEntity", typeof(T).Name);
        }
    }


    public async Task DeleteAsync(T entity)
    {
        this._logger.LogInformation("Soft deleting entity of type {EntityType}", typeof(T).Name);

        if (entity is BaseEntity)
        {
            await this._dbSet
                .Where(e => e.Equals(entity))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => (e as BaseEntity).IsDeleted, false)
                );

            this._logger.LogInformation("Entity of type {EntityType} soft deleted", typeof(T).Name);

            await this.CommitAsync();
        }
        else
        {
            this._logger.LogWarning("Entity of type {EntityType} does not inherit from BaseEntity", typeof(T).Name);
        }
    }


    #endregion

    #region Sync methods

    public void AddRange(IEnumerable<T> entity)
    {
        this._logger.LogInformation("Adding range entity of type {EntityType}", typeof(T).Name);
        this._dbSet.AddRange(entity);
        this._logger.LogInformation("Entity of type {EntityType} added", typeof(T).Name);

        this.Commit();
    }

    public virtual void Update(T entity)
    {
        this._logger.LogInformation("Updating entity of type {EntityType}", typeof(T).Name);
        this._dbSet.Update(entity);
        this._logger.LogInformation("Entity of type {EntityType} updated");

        this.Commit();
    }

    public virtual void Delete(T entity)
    {
        this._logger.LogInformation("Soft deleting entity of type {EntityType}", typeof(T).Name);

        if (entity is BaseEntity)
        {
            this._dbSet
                .Where(e => e.Equals(entity))
                .ExecuteUpdate(setters => setters
                    .SetProperty(e => (e as BaseEntity).IsDeleted, true)
                );

            this._logger.LogInformation("Entity of type {EntityType} soft deleted", typeof(T).Name);

            this.Commit();
        }
        else
        {
            this._logger.LogWarning("Entity of type {EntityType} does not inherit from BaseEntity", typeof(T).Name);
        }
    }



    public T GetById(Guid id)
    {
        this._logger.LogInformation("Getting entity of type {EntityType} by id {Id}", typeof(T).Name, id);

        T entity;

        entity = this._dbSet.Find(id);

        if (entity == null)
        {
            this._logger.LogWarning("Entity of type {EntityType} with id {Id} not found", typeof(T).Name, id);
        }
        else
        {
            this._logger.LogInformation("Entity of type {EntityType} with id {Id} found in database", typeof(T).Name, id);
        }

        return entity;
    }

    public virtual int Commit()
    {
        this._logger.LogInformation("Committing transaction");
        var result = this._context.SaveChanges();
        this._logger.LogInformation("Transaction committed");
        return result;
    }

    public void Add(T entity)
    {
        this._logger.LogInformation("Adding entity of type {EntityType}", typeof(T).Name);
        this._dbSet.Add(entity);
        this._logger.LogInformation("Entity of type {EntityType} added");

        this.Commit();
    }

    #endregion

 

    public virtual void BeginTransaction()
    {
        this._logger.LogInformation("Beginning transaction");
        this._context.Database.BeginTransaction();
        this._logger.LogInformation("Transaction started");
    }

    public virtual void CommitTransaction()
    {
        this._logger.LogInformation("Committing transaction");
        this._context.Database.CommitTransaction();
        this._logger.LogInformation("Transaction committed");
    }

    public virtual void RollbackTransaction()
    {
        this._logger.LogInformation("Rolling back transaction");
        this._context.Database.RollbackTransaction();
        this._logger.LogInformation("Transaction rolled back");
    }
}
