namespace Api.DomainModels.GenericResult;

public class Result<T>
{
    public bool IsSuccess { get; set; }

    public T? Value { get; set; }

    public string? Error { get; set; }

    public ErrorType?  ErrorType { get; set; }

    public static Result<T> Success(T value)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = value
        };
    }

    public static Result<T> Failure(ErrorType errorType)
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorType = errorType
        };
    }

    public static Result<T> Failure(string error, ErrorType errorType)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Error = error,
            ErrorType = errorType
        };
    }
}

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Unauthorized,
    Unknown,
    BUSINESS_LOGIC
}