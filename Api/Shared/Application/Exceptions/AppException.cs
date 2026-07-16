namespace Api.Shared.Application.Exceptions;

public abstract class AppException : Exception
{
    public ErrorType ErrorType { get; init; }

    public AppException(string message, ErrorType errorType)
        : base(message)
    {
        ErrorType = errorType;
    }
}