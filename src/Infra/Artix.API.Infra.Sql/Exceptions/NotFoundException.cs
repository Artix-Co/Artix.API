namespace Artix.API.Infra.Sql.Exceptions;


public sealed class NotFoundException : Exception
{
    private NotFoundException(string message) : base(message) { }

    public static NotFoundException ForEntity(string entityName, object key) =>
        new($"Entity '{entityName}' with key '{key}' was not found.");

    public static NotFoundException ForResource(string resource) =>
        new($"Resource '{resource}' was not found.");

    public static NotFoundException WithMessage(string message) =>
        new(message);
}
