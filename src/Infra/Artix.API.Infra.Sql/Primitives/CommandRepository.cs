namespace Artix.API.Infra.Sql.Primitives;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data;
using EFCore.BulkExtensions;

public class CommandRepository<T>(ArtixCommandDbContext commandDbContext)
    : ICommandRepository<T> where T : class, IAggregateRoot, IEntity
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
        if (entity is BaseEntity baseEntity)
            baseEntity.ApplyGraphTracking(_commandDbContext);

        _commandDbContext.SaveChanges();
    }

    public void Delete(long id)
    {
        var dbSet = this._commandDbContext.Set<T>();
        var entity = dbSet.Find(id);

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

    #endregion

    #region Async Methods

    public async Task InsertRangeAsync(IEnumerable<T> entities)
    {
        await this._commandDbContext.BulkInsertAsync(entities);
    }


    public async Task InsertAsync(T entity)
    {
        await this._commandDbContext.Set<T>().AddAsync(entity);
        await this._commandDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity is BaseEntity baseEntity)
            baseEntity.ApplyGraphTracking(_commandDbContext);

        await _commandDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var dbSet = this._commandDbContext.Set<T>();
        var entity = dbSet.Find(id);

        var entityType = typeof(T);

        var isActiveProperty = entityType.GetProperty("IsActive");
        if (isActiveProperty == null)
            throw new InvalidOperationException($"Entity {entityType.Name} does not have an 'IsActive' property.");


        isActiveProperty.SetValue(entity, false);


        this._commandDbContext.Update(entity);
        await this._commandDbContext.SaveChangesAsync();
    }

    #endregion
}
