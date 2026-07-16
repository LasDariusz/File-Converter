using Api.Shared.Application.Exceptions;

namespace Api.Modules.Users.Application.Exceptions;

public class InvalidCredentialsException : AppException
{
    public InvalidCredentialsException() : base(
        "Provided credentials are invalid",
        ErrorType.Unauthorized)
    {
    }
}