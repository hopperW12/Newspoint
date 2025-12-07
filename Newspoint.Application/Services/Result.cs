using System.Text.Json.Serialization;

namespace Newspoint.Application.Services;

public class Result
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    [JsonIgnore]
    public ResultErrorType ErrorType { get; set; }

    public static Result Ok() => new() { Success = true };
    public static Result Error(ResultErrorType errorType, string message) => new() { ErrorType = errorType, Success = false, Message = message };
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Ok(T data) => new() { Success = true, Data = data };
    public new static Result<T> Error(ResultErrorType errorType, string message) => new() { ErrorType = errorType, Success = false, Message = message };
}

public enum ResultErrorType
{
    NotFound,
    Validation,
    UnknownError
}