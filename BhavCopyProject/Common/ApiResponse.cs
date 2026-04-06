namespace BhavCopyProject.Common;

public abstract record ApiResponseBase
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = "OK";
}

public sealed record ApiResponse<T> : ApiResponseBase
{
    public T? Data { get; init; }
    public object? Errors { get; init; }

    public static ApiResponse<T> Ok(T? data, int statusCode = 200, string message = "OK")
        => new() { Success = true, StatusCode = statusCode, Message = message, Data = data };

    public static ApiResponse<T> Fail(object? errors, int statusCode, string message)
        => new() { Success = false, StatusCode = statusCode, Message = message, Errors = errors };
}

