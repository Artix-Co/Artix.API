namespace Artix.API.Core.Domain.Exceptions;

public sealed class DomainException : Exception
{
    private DomainException(string message) : base(message)
    {
    }

    public static DomainException BusinessRuleViolation(string message) =>
        new(message);

    public static Exception NotFound(string collection, long collectionId)=>    new($"Collection '{collection}' with key '{collectionId}' was not found.");
}
