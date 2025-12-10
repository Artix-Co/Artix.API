namespace Artix.API.Core.Contract.Primitives.Models;
public sealed class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }

    private Result(bool isSuccess, T? data = default, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T data)
        => new Result<T>(true, data);

    public static Result<T> Failure(string errorMessage)
        => new Result<T>(false, default, errorMessage);

    public override string ToString()
        => IsSuccess
            ? $"Success: {Data}"
            : $"Failure: {ErrorMessage}";
}
