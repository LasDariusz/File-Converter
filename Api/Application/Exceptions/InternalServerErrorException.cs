using System.Net;

namespace Api.Application.Exceptions;

public sealed class InternalServerErrorException : AppException
{
    public InternalServerErrorException(string message = "Internal server error") 
        : base(message, HttpStatusCode.InternalServerError)
    {
    }
}