using System.Net;

namespace Api.Application.Exceptions;

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message = "Bad request") 
        : base(message, HttpStatusCode.BadRequest)
    {
    }
}