namespace Artix.API.Core.Contract.Primitives.Models;
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }

    private Result(bool isSuccess, T? data = default, string? errorMessage = null, Exception? exception = null)
    {
        this.IsSuccess = isSuccess;
        this.Data = data;
        this.ErrorMessage = errorMessage;
        this.Exception = exception;
    }

    public static Result<T> Success(T data) => new Result<T>(true, data);

    public static Result<T> Failure(string errorMessage, Exception? exception = null)
        => new Result<T>(false, default, errorMessage, exception);

    public override string ToString()
    {
        return this.IsSuccess
            ? $"Success: {this.Data}"
            : $"Failure: {this.ErrorMessage}, Exception: {this.Exception?.Message}";
    }
}
