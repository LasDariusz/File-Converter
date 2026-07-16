using System.Net;

namespace Api.Application.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string resource, string key) 
        : base($"{resource} with key '{key}' was not found", HttpStatusCode.NotFound)
    {
    }
}