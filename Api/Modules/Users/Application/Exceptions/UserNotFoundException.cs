using Api.Shared.Application.Exceptions;

namespace Api.Modules.Users.Application.Exceptions;

public class UserNotFoundException : AppException
{
    public UserNotFoundException() : base(
        "User does not exist",
        ErrorType.NotFound)
    {
    }
}