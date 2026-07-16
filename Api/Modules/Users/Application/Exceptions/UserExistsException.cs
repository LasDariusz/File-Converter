using Api.Shared.Application.Exceptions;

namespace Api.Modules.Users.Application.Exceptions;

public class UserExistsException : AppException
{
    public UserExistsException() : base(
        "User already exists",
        ErrorType.Conflict)
    {
    }
}