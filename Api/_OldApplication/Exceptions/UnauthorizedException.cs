using System.Net;

namespace Api.Application.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Invalid credentials") 
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}