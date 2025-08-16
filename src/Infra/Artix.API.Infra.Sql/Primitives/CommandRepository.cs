namespace Artix.API.Infra.Sql.Primitives;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data;
using Data.DbContexts;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

public class CommandRepository<T>(ArtixCommandDbContext commandDbContext)
    : ICommandRepository<T> where T : AggregateRoot
{
    protected readonly ArtixCommandDbContext _commandDbContext = commandDbContext;

    #region Sync Methods

    public void InsertRange(IEnumerable<T> entities)
    {
        this._commandDbContext.BulkInsert(entities);
    }


    public void Insert(T entity)
    {
        this._commandDbContext.Set<T>().Attach(entity);
        this._commandDbContext.SaveChanges();
    }

    public void Update(T entity)
    {
        _commandDbContext.SaveChanges();
    }

    public void Delete(Guid businessId)
    {
        var dbSet = this._commandDbContext.Set<T>();
        var entity = dbSet.FirstOrDefault(entity => entity.BusinessId == businessId);

        var entityType = typeof(T);

        // Ensure the entity has an "IsActive" property before updating
        var isActiveProperty = entityType.GetProperty("IsActive");
        if (isActiveProperty == null)
            throw new InvalidOperationException($"Entity {entityType.Name} does not have an 'IsActive' property.");

        // Set "IsActive" to false
        isActiveProperty.SetValue(entity, false);

        // Perform soft delete
        this._commandDbContext.Update(entity);
        this._commandDbContext.SaveChanges();
    }

    public T? GetById(Guid businessId)
    {
        return _commandDbContext.Set<T>().FirstOrDefault(entity => entity.BusinessId == businessId);
    }

    #endregion

    #region Async Methods

    public async Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await this._commandDbContext.BulkInsertAsync(entities);
    }


    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        await this._commandDbContext.Set<T>().AddAsync(entity, cancellationToken);
        await this._commandDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _commandDbContext.SaveChangesAsync(cancellationToken);
    }


    public async Task<T?> GetByIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        return await _commandDbContext.Set<T>()
            .FirstOrDefaultAsync(entity => entity.BusinessId == businessId, cancellationToken);
    }


    public async Task DeleteAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        var dbSet = this._commandDbContext.Set<T>();
        var entity = await dbSet.FirstOrDefaultAsync(entity => entity.BusinessId == businessId, cancellationToken);

        var entityType = typeof(T);

        var isActiveProperty = entityType.GetProperty("IsActive");
        if (isActiveProperty == null)
            throw new InvalidOperationException($"Entity {entityType.Name} does not have an 'IsActive' property.");


        isActiveProperty.SetValue(entity, false);


        this._commandDbContext.Update(entity);
        await this._commandDbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
