namespace Artix.API.Core.Contract.Primitives.Models;

public class BaseApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
}
