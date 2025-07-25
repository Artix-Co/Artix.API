namespace Artix.API.Core.Domain.Exceptions;

public sealed class DomainException : Exception
{
    private DomainException(string message) : base(message)
    {
    }

    public static DomainException BusinessRuleViolation(string message) =>
        new(message);

    public static Exception NotFound(string collection, long collectionId)=>    new($"Collection '{collection}' with key '{collectionId}' was not found.");
    public static DomainException InvalidValue(string fieldName) =>
        new($"Field '{fieldName}' cannot be null, empty, or whitespace.");
    
    public static DomainException InvalidOperation(string operation) =>
        new($"Invalid operation: {operation}.");
}
