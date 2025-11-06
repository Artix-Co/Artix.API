namespace Artix.API.Core.ApplicationService.Exceptions;

public sealed class TooManyRequestsException : ApplicationException
{
    public TooManyRequestsException(string message) : base(message) { }
}
