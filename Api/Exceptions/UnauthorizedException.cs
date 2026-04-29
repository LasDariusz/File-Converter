using System.Net;

namespace Api.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Invalid credentials") 
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
