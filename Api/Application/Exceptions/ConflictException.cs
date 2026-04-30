using System.Net;

namespace Api.Application.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string resource, string key) 
        : base($"{resource} with key '{key}' already exists", HttpStatusCode.Conflict)
    {
    }
}