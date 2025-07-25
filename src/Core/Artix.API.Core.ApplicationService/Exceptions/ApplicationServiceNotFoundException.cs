namespace Artix.API.Core.ApplicationService.Exceptions;


public sealed class ApplicationServiceNotFoundException : Exception
{
    private ApplicationServiceNotFoundException(string message) : base(message) { }

    public static ApplicationServiceNotFoundException ForEntity(string entityName, object key) =>
        new($"Entity '{entityName}' with key '{key}' was not found.");

    public static ApplicationServiceNotFoundException ForResource(string resource) =>
        new($"Resource '{resource}' was not found.");

    public static ApplicationServiceNotFoundException WithMessage(string message) =>
        new(message);
}
