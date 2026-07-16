using System.Net;

namespace Api.Application.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; set; }

    public AppException(string message,  HttpStatusCode statusCode) 
        : base(message)
    {
        StatusCode = statusCode; 
    }

}