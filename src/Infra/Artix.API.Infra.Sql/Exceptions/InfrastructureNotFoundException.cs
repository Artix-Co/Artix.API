namespace Artix.API.Infra.Sql.Exceptions;


public sealed class InfrastructureNotFoundException : Exception
{
    private InfrastructureNotFoundException(string message) : base(message) { }

    public static InfrastructureNotFoundException ForEntity(string entityName, object key) =>
        new($"Entity '{entityName}' with key '{key}' was not found.");

    public static InfrastructureNotFoundException ForResource(string resource) =>
        new($"Resource '{resource}' was not found.");

    public static InfrastructureNotFoundException WithMessage(string message) =>
        new(message);
}
