using System.Net;

namespace Api.Application.Exceptions;

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Access forbidden")
        : base(message, HttpStatusCode.Forbidden)
    {
    }
}