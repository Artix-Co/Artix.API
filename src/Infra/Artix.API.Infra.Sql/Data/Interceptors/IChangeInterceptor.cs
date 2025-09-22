namespace Artix.API.Infra.Sql.Data.Interceptors;

using DbContexts;

public interface IChangeInterceptor
{
    void BeforeSaveChanges(ArtixCommandDbContext context);
    Task BeforeSaveChangesAsync(ArtixCommandDbContext context, CancellationToken cancellationToken);
}
